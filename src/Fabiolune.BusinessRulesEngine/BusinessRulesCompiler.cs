using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Fabiolune.BusinessRulesEngine.Interfaces;
using Fabiolune.BusinessRulesEngine.Models;
using Newtonsoft.Json;
using Serilog;

namespace Fabiolune.BusinessRulesEngine
{
    public class BusinessRulesCompiler : IBusinessRulesCompiler
    {
        private const string Component = nameof(BusinessRulesCompiler);
        private static readonly Expression NullValue = Expression.Constant(null);
        private static readonly Type ResultType = typeof(RuleApplicationResult);
        private readonly PropertyInfo _codesPropertyInfo;
        private readonly ILogger _logger;
        private readonly PropertyInfo _successPropertyInfo;

        public BusinessRulesCompiler(ILogger logger)
        {
            _logger = logger;
            _successPropertyInfo = ResultType.GetProperty(nameof(RuleApplicationResult.Success));
            _codesPropertyInfo = ResultType.GetProperty(nameof(RuleApplicationResult.Code));
        }

        public IEnumerable<Func<T, RuleApplicationResult>> CompileRules<T>(IEnumerable<Rule> rules) 
            => rules
                .Select(r => GenerateFunc<T>(CreateCompiledRule<T>(r)))
                .Where(r => r != null);

        private Func<T, RuleApplicationResult> GenerateFunc<T>(ExpressionTypeCodeBinding pair)
        {
            if (pair?.BoolExpression == null)
                return null;

            return Expression.Lambda<Func<T, RuleApplicationResult>>(string.IsNullOrEmpty(pair.Code)
                ? Expression.MemberInit(
                    Expression.New(ResultType),
                    Expression.Bind(_successPropertyInfo, pair.BoolExpression))
                : Expression.MemberInit(
                    Expression.New(ResultType),
                    Expression.Bind(_successPropertyInfo, pair.BoolExpression),
                    Expression.Bind(_codesPropertyInfo, Expression.Constant(pair.Code))
                ), pair.TypeExpression).Compile();
        }

        private ExpressionTypeCodeBinding CompileDirectRule<T>(Rule rule)
        {
            try
            {
                var mapping = new Dictionary<OperatorType, ExpressionType>
                {
                    {OperatorType.Equal, ExpressionType.Equal},
                    {OperatorType.GreaterThan, ExpressionType.GreaterThan},
                    {OperatorType.GreaterThanOrEqual, ExpressionType.GreaterThanOrEqual},
                    {OperatorType.LessThan, ExpressionType.LessThan},
                    {OperatorType.LessThanOrEqual, ExpressionType.LessThanOrEqual},
                    {OperatorType.NotEqual, ExpressionType.NotEqual}
                };

                var genericType = Expression.Parameter(typeof(T));
                var propertyType = typeof(T).GetProperty(rule.Property).PropertyType;

                var value = Expression.Constant(propertyType.BaseType == typeof(Enum)
                    ? Enum.Parse(propertyType, rule.Value)
                    : Convert.ChangeType(rule.Value, propertyType));

                return new ExpressionTypeCodeBinding
                {
                    BoolExpression = Expression.MakeBinary(mapping[rule.Operator],
                        Expression.Property(genericType, rule.Property), value),
                    TypeExpression = genericType,
                    Code = rule.Code
                };
            }
            catch (Exception e)
            {
                _logger.Error(e, "{Component} raised an exception with {Message} when compiling {Rule}", Component,
                    e.Message, JsonConvert.SerializeObject(rule, Formatting.Indented));
                return null;
            }
        }

        private ExpressionTypeCodeBinding CompileInternalDirectRule<T>(Rule rule)
        {
            const string method = nameof(CompileInternalDirectRule);

            var mapping = new Dictionary<OperatorType, ExpressionType>
            {
                {OperatorType.InnerEqual, ExpressionType.Equal},
                {OperatorType.InnerGreaterThan, ExpressionType.GreaterThan},
                {OperatorType.InnerGreaterThanOrEqual, ExpressionType.GreaterThanOrEqual},
                {OperatorType.InnerLessThan, ExpressionType.LessThan},
                {OperatorType.InnerLessThanOrEqual, ExpressionType.LessThanOrEqual},
                {OperatorType.InnerNotEqual, ExpressionType.NotEqual}
            };

            try
            {
                var expressionType = mapping[rule.Operator];

                var genericType = Expression.Parameter(typeof(T));
                var key = Expression.Property(genericType, rule.Property);
                var propertyType = typeof(T).GetProperty(rule.Property).PropertyType;
                var type1 = propertyType.FullName;

                var key2 = Expression.Property(genericType, rule.Value);
                var propertyType2 = typeof(T).GetProperty(rule.Value).PropertyType;
                var type2 = propertyType2.FullName;

                if (type1 != type2)
                {
                    _logger.Error(
                        "{Component} {Operation}: {Property1} is of type {Type1} while {Property2} is of type {Type2}, no direct comparison possible",
                        Component, method, propertyType, type1, propertyType2, type2);
                    return null;
                }

                return new ExpressionTypeCodeBinding
                {
                    BoolExpression = Expression.MakeBinary(expressionType, key, key2),
                    TypeExpression = genericType,
                    Code = rule.Code
                };
            }
            catch (Exception e)
            {
                _logger.Error(e, "{Component} raised an exception with {Message} when compiling {Rule}", Component,
                    e.Message, JsonConvert.SerializeObject(rule, Formatting.Indented));
                return null;
            }
        }

        private ExpressionTypeCodeBinding CompileEnumerableRule<T>(Rule rule)
        {
            try
            {
                var genericType = Expression.Parameter(typeof(T));
                var key = Expression.Property(genericType, rule.Property);
                var propertyType = typeof(T).GetProperty(rule.Property).PropertyType;
                var searchValuesType = propertyType.IsArray
                    ? propertyType.GetElementType()
                    : propertyType.GetGenericArguments().FirstOrDefault();

                var mapping = new Dictionary<OperatorType, Func<Rule, MemberExpression, Type, BinaryExpression>>
                {
                    {
                        OperatorType.Contains, (r, k, s) => Expression.MakeBinary(ExpressionType.AndAlso,
                            Expression.MakeBinary(ExpressionType.NotEqual, k, NullValue),
                            Expression.Call(typeof(Enumerable), nameof(Enumerable.Contains), new[] {s},
                                key, Expression.Constant(Convert.ChangeType(r.Value, s))))
                    },
                    {
                        OperatorType.NotContains, (r, k, s) => Expression.MakeBinary(ExpressionType.OrElse,
                            Expression.MakeBinary(ExpressionType.Equal, k, NullValue),
                            Expression.IsFalse(Expression.Call(typeof(Enumerable), nameof(Enumerable.Contains),
                                new[] {s}, k,
                                Expression.Constant(Convert.ChangeType(r.Value, s)))))
                    },
                    {
                        OperatorType.Overlaps, (r, k, s) =>
                        {
                            var ae = Expression.NewArrayInit(s,
                                r.Value.Split(',').Select(v => Convert.ChangeType(v, s, CultureInfo.InvariantCulture))
                                    .Select(Expression.Constant));
                            return Expression.MakeBinary(ExpressionType.AndAlso,
                                Expression.MakeBinary(ExpressionType.AndAlso,
                                    Expression.MakeBinary(ExpressionType.NotEqual, k, NullValue),
                                    Expression.MakeBinary(ExpressionType.NotEqual, ae, NullValue)),
                                Expression.IsTrue(Expression.Call(typeof(Enumerable), nameof(Enumerable.Any), new[] {s},
                                    Expression.Call(typeof(Enumerable), nameof(Enumerable.Intersect), new[] {s}, k,
                                        ae))));
                        }
                    },
                    {
                        OperatorType.NotOverlaps, (r, k, s) =>
                        {
                            var ae = Expression.NewArrayInit(s,
                                r.Value.Split(',').Select(v => Convert.ChangeType(v, s, CultureInfo.InvariantCulture))
                                    .Select(Expression.Constant));
                            return Expression.MakeBinary(ExpressionType.OrElse,
                                Expression.MakeBinary(ExpressionType.OrElse,
                                    Expression.MakeBinary(ExpressionType.Equal, k, NullValue),
                                    Expression.MakeBinary(ExpressionType.Equal, ae, NullValue)),
                                Expression.IsFalse(Expression.Call(typeof(Enumerable), nameof(Enumerable.Any),
                                    new[] {s},
                                    Expression.Call(typeof(Enumerable), nameof(Enumerable.Intersect), new[] {s}, key,
                                        ae))));
                        }
                    }
                };

                return new ExpressionTypeCodeBinding
                {
                    BoolExpression = mapping[rule.Operator](rule, key, searchValuesType),
                    TypeExpression = genericType,
                    Code = rule.Code
                };
            }
            catch (Exception e)
            {
                _logger.Error(e, "{Component} raised an exception with {Message} when compiling {Rule}", Component,
                    e.Message, JsonConvert.SerializeObject(rule, Formatting.Indented));
                return null;
            }
        }

        private ExpressionTypeCodeBinding CompileInternalEnumerableRule<T>(Rule rule)
        {
            const string method = nameof(CompileInternalDirectRule);
            try
            {
                var genericType = Expression.Parameter(typeof(T));

                var key = Expression.Property(genericType, rule.Property);
                var propertyType = typeof(T).GetProperty(rule.Property).PropertyType;

                var key2 = Expression.Property(genericType, rule.Value);
                var propertyType2 = typeof(T).GetProperty(rule.Value).PropertyType;

                var mapping = new Dictionary<OperatorType, Func<Rule, MemberExpression, Type, MemberExpression, Type, BinaryExpression>>
                {
                    { OperatorType.InnerContains,  (r, k, pt, k2, pt2) =>
                    {
                        var svt = pt.IsArray ? pt.GetElementType() : pt.GetGenericArguments().FirstOrDefault();
                        if (svt.FullName != pt2.FullName)
                        {
                            _logger.Error(
                                "{Component} {Operation}: {Property1} is of type IEnumerable[{Type1}] while {Property2} is of type {Type2}, no comparison possible",
                                Component, method, pt, svt.FullName, pt2, pt2.FullName);
                            return null;
                        }
                        return Expression.MakeBinary(ExpressionType.AndAlso, Expression.MakeBinary(ExpressionType.NotEqual, k, NullValue), Expression.Call(typeof(Enumerable), nameof(Enumerable.Contains), new[] { svt }, k, k2));
                    }},
                    { OperatorType.InnerNotContains, (r, k, pt, k2, pt2) =>
                    {
                        var svt = pt.IsArray ? pt.GetElementType() : pt.GetGenericArguments().FirstOrDefault();
                        if (svt.FullName != pt2.FullName)
                        {
                            _logger.Error(
                                "{Component} {Operation}: {Property1} is of type IEnumerable[{Type1}] while {Property2} is of type {Type2}, no comparison possible",
                                Component, method, pt, svt.FullName, pt2, pt2.FullName);
                            return null;
                        }
                        return Expression.MakeBinary(ExpressionType.OrElse, Expression.MakeBinary(ExpressionType.Equal, k, NullValue), Expression.IsFalse(Expression.Call(typeof(Enumerable), nameof(Enumerable.Contains), new[] { svt }, k, k2)));
                    } },
                    { OperatorType.InnerOverlaps, (r, k, pt, k2, pt2) =>
                    {
                        var svt = pt.IsArray ? pt.GetElementType() : pt.GetGenericArguments().FirstOrDefault();
                        if (pt != pt2)
                        {
                            _logger.Error(
                                "{Component} {Operation}: {Property1} is of type {PropertyType1} while {Property2} is of type {PropertyType2}, no comparison possible",
                                Component, method, pt, pt, pt2, pt2.FullName);
                            return null;
                        }
                        return Expression.MakeBinary(ExpressionType.AndAlso, Expression.MakeBinary(
                            ExpressionType.AndAlso,
                            Expression.MakeBinary(ExpressionType.NotEqual, k, NullValue),
                            Expression.MakeBinary(ExpressionType.NotEqual, k2, NullValue)
                        ), Expression.IsTrue(Expression.Call(
                            typeof(Enumerable),
                            nameof(Enumerable.Any),
                            new[] { svt },
                            Expression.Call(typeof(Enumerable), nameof(Enumerable.Intersect), new[] { svt },
                                k, k2)
                        )));
                    } },
                    { OperatorType.InnerNotOverlaps, (r, k, pt, k2, pt2) =>
                    {
                        var svt = pt.IsArray ? pt.GetElementType() : pt.GetGenericArguments().FirstOrDefault();
                        if (pt != pt2)
                        {
                            _logger.Error(
                                "{Component} {Operation}: {Property1} is of type {PropertyType1} while {Property2} is of type {PropertyType2}, no comparison possible",
                                Component, method, pt, pt, pt2, pt2.FullName);
                            return null;
                        }

                        return Expression.MakeBinary(ExpressionType.OrElse, Expression.MakeBinary(
                            ExpressionType.OrElse,
                            Expression.MakeBinary(ExpressionType.Equal, k, NullValue),
                            Expression.MakeBinary(ExpressionType.Equal, k2, NullValue)
                        ), Expression.IsFalse(Expression.Call(
                            typeof(Enumerable),
                            nameof(Enumerable.Any),
                            new[] { svt },
                            Expression.Call(typeof(Enumerable), nameof(Enumerable.Intersect), new[] { svt }, k, k2)
                        )));
                    } }
                };

                return new ExpressionTypeCodeBinding
                {
                    BoolExpression = mapping[rule.Operator](rule, key, propertyType, key2, propertyType2),
                    TypeExpression = genericType,
                    Code = rule.Code
                };
            }
            catch (Exception e)
            {
                _logger.Error(e, "{Component} raised an exception with {Message} when compiling {Rule}", Component,
                    e.Message, JsonConvert.SerializeObject(rule, Formatting.Indented));
                return null;
            }
        }

        private ExpressionTypeCodeBinding CompileExternalEnumerableRule<T>(Rule rule)
        {
            try
            {
                var genericType = Expression.Parameter(typeof(T));

                var propertyType = typeof(T).GetProperty(rule.Property).PropertyType;

                var mapping =
                    new Dictionary<OperatorType, Func<MemberExpression, Type, NewArrayExpression, BinaryExpression>>
                    {
                        {
                            OperatorType.IsContained,
                            (k, p, ae) => Expression.MakeBinary(ExpressionType.AndAlso,
                                Expression.MakeBinary(ExpressionType.NotEqual, ae, NullValue),
                                Expression.Call(typeof(Enumerable), nameof(Enumerable.Contains), new[] {p}, ae, k))
                        },
                        {
                            OperatorType.IsNotContained,
                            (k, p, ae) => Expression.MakeBinary(ExpressionType.OrElse,
                                Expression.MakeBinary(ExpressionType.Equal, ae, NullValue),
                                Expression.IsFalse(Expression.Call(typeof(Enumerable), nameof(Enumerable.Contains),
                                    new[] {p}, ae, k)))
                        }
                    };

                return new ExpressionTypeCodeBinding
                {
                    BoolExpression = mapping[rule.Operator](Expression.Property(genericType, rule.Property),
                        propertyType, Expression.NewArrayInit(propertyType, rule.Value.Split(',')
                            .Select(v => Convert.ChangeType(v, propertyType, CultureInfo.InvariantCulture))
                            .Select(Expression.Constant))),
                    TypeExpression = genericType,
                    Code = rule.Code
                };
            }
            catch (Exception e)
            {
                _logger.Error(e, "{Component} raised an exception with {Message} when compiling {Rule}", Component,
                    e.Message, JsonConvert.SerializeObject(rule, Formatting.Indented));
                return null;
            }
        }

        private ExpressionTypeCodeBinding CompileExternalKeyValueRule<T>(Rule rule)
        {
            try
            {
                var type = typeof(T);
                var genericType = Expression.Parameter(type);
                var getItemMethodInfo = typeof(Dictionary<string, string>).GetMethod("get_Item");

                var mapping = new Dictionary<OperatorType, Func<ParameterExpression, Rule, Type, BinaryExpression>>
                {
                    {
                        OperatorType.ContainsKey, (g, r, t) =>
                        {
                            var p = Expression.Property(g, r.Property);
                            return Expression.MakeBinary(ExpressionType.AndAlso,
                                Expression.MakeBinary(ExpressionType.NotEqual, p, NullValue),
                                Expression.Call(p, typeof(IDictionary<string, string>).GetMethod("ContainsKey"),
                                    Expression.Constant(Convert.ChangeType(r.Value,
                                        t.GetProperty(r.Property).PropertyType.GetGenericArguments()[0]))));
                        }
                    },
                    {
                        OperatorType.NotContainsKey, (g, r, t) =>
                        {
                            var property = Expression.Property(g, r.Property);
                            return Expression.MakeBinary(ExpressionType.OrElse,
                                Expression.MakeBinary(ExpressionType.Equal, property, NullValue), Expression.IsFalse(
                                    Expression.Call(property,
                                        typeof(IDictionary<string, string>).GetMethod("ContainsKey"),
                                        Expression.Constant(Convert.ChangeType(r.Value,
                                            t.GetProperty(r.Property).PropertyType.GetGenericArguments()[0])))));
                        }
                    },
                    {
                        OperatorType.ContainsValue, (g, r, t) =>
                        {
                            var p = Expression.Property(g, r.Property);
                            return Expression.MakeBinary(ExpressionType.AndAlso,
                                Expression.MakeBinary(ExpressionType.NotEqual, p, NullValue), Expression.Call(p,
                                    typeof(Dictionary<string, string>).GetMethod("ContainsValue"),
                                    Expression.Constant(Convert.ChangeType(r.Value,
                                        t.GetProperty(r.Property).PropertyType.GetGenericArguments()[1]))));
                        }
                    },
                    {
                        OperatorType.NotContainsValue, (p, r, t) =>
                        {
                            var property = Expression.Property(p, r.Property);
                            return Expression.MakeBinary(ExpressionType.OrElse,
                                Expression.MakeBinary(ExpressionType.Equal, property, NullValue), Expression.IsFalse(
                                    Expression.Call(property,
                                        typeof(Dictionary<string, string>).GetMethod("ContainsValue"),
                                        Expression.Constant(Convert.ChangeType(r.Value,
                                            t.GetProperty(r.Property).PropertyType.GetGenericArguments()[0])))));
                        }
                    },
                    {
                        OperatorType.KeyContainsValue, (p, r, t) =>
                        {
                            var parts = r
                                .Property
                                .Split("[".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                                .Select(_ => _.TrimEnd(']')).ToArray();
                            var parameters = type.GetProperty(parts[0]).PropertyType.GetGenericArguments();
                            var pp = Expression.Property(p, parts[0]);
                            return Expression.MakeBinary(ExpressionType.AndAlso, Expression.MakeBinary(
                                    ExpressionType.AndAlso,
                                    Expression.MakeBinary(ExpressionType.NotEqual, pp, NullValue),
                                    Expression.Call(pp, typeof(IDictionary<string, string>).GetMethod("ContainsKey"),
                                        Expression.Constant(Convert.ChangeType(parts[1], parameters[0])))),
                                Expression.MakeBinary(ExpressionType.Equal, Expression.Call(pp, getItemMethodInfo,
                                        Expression.Constant(Convert.ChangeType(parts[1], parameters[0]))),
                                    Expression.Constant(Convert.ChangeType(r.Value, parameters[1]))));
                        }
                    },
                    {
                        OperatorType.NotKeyContainsValue, (p, r, t) =>
                        {
                            var parts = r
                                .Property
                                .Split("[".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                                .Select(_ => _.TrimEnd(']')).ToArray();
                            var parameters = type.GetProperty(parts[0]).PropertyType.GetGenericArguments();
                            var pp = Expression.Property(p, parts[0]);
                            return Expression.MakeBinary(ExpressionType.OrElse, Expression.MakeBinary(
                                    ExpressionType.OrElse, Expression.MakeBinary(ExpressionType.Equal, pp, NullValue),
                                    Expression.IsFalse(Expression.Call(pp,
                                        typeof(IDictionary<string, string>).GetMethod("ContainsKey"),
                                        Expression.Constant(Convert.ChangeType(parts[1], parameters[0]))))),
                                Expression.IsFalse(Expression.MakeBinary(ExpressionType.Equal, Expression.Call(pp,
                                        getItemMethodInfo,
                                        Expression.Constant(Convert.ChangeType(parts[1], parameters[0]))),
                                    Expression.Constant(Convert.ChangeType(r.Value, parameters[1])))));
                        }
                    }
                };

                return new ExpressionTypeCodeBinding
                {
                    BoolExpression = mapping[rule.Operator](genericType, rule, type),
                    TypeExpression = genericType,
                    Code = rule.Code
                };
            }
            catch (Exception e)
            {
                _logger.Error(e, "{Component} raised an exception with {Message} when compiling {Rule}", Component,
                    e.Message, JsonConvert.SerializeObject(rule, Formatting.Indented));
                return null;
            }
        }

        private ExpressionTypeCodeBinding CreateCompiledRule<T>(Rule rule)
        {
            switch (OperatorClassification.GetOperatorType(rule.Operator))
            {
                case OperatorClassification.OperatorCategory.Direct:
                    return CompileDirectRule<T>(rule);
                case OperatorClassification.OperatorCategory.Enumerable:
                    return CompileEnumerableRule<T>(rule);
                case OperatorClassification.OperatorCategory.InternalDirect:
                    return CompileInternalDirectRule<T>(rule);
                case OperatorClassification.OperatorCategory.InternalEnumerable:
                    return CompileInternalEnumerableRule<T>(rule);
                case OperatorClassification.OperatorCategory.ExternalEnumerable:
                    return CompileExternalEnumerableRule<T>(rule);
                case OperatorClassification.OperatorCategory.KeyValue:
                    return CompileExternalKeyValueRule<T>(rule);
                default:
                    return null;
            }
        }
    }
}