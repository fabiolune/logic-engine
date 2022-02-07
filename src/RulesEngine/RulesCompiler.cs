using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using RulesEngine.Interfaces;
using RulesEngine.Internals;
using RulesEngine.Models;
using Serilog;
using TinyFp;
using TinyFp.Extensions;
using static TinyFp.Prelude;

namespace RulesEngine
{
    public class RulesCompiler : IRulesCompiler
    {
        private const string Component = nameof(RulesCompiler);
        private static readonly Type ResultType = typeof(RuleApplicationResult);
        private readonly PropertyInfo _codesPropertyInfo;
        private readonly ILogger _logger;
        private readonly PropertyInfo _successPropertyInfo;

        public RulesCompiler(ILogger logger)
        {
            _logger = logger;
            _successPropertyInfo = ResultType.GetProperty(nameof(RuleApplicationResult.Success));
            _codesPropertyInfo = ResultType.GetProperty(nameof(RuleApplicationResult.Code));
        }

        public IEnumerable<Func<T, RuleApplicationResult>> CompileRules<T>(IEnumerable<Rule> rules)
            => rules
                .Select(r => GenerateFunc<T>(CreateCompiledRule<T>(r)))
                .Where(r => r.IsSome)
                .Select(f => f.Match(_ => _, () => _ => new RuleApplicationResult()));

        private Option<Func<T, RuleApplicationResult>> GenerateFunc<T>(Option<ExpressionTypeCodeBinding> pair) =>
            pair
                .Map(_ => (string.IsNullOrEmpty(_.Code)
                    ? Expression.MemberInit(
                        Expression.New(ResultType),
                        Expression.Bind(_successPropertyInfo, _.BoolExpression))
                    : Expression.MemberInit(
                        Expression.New(ResultType),
                        Expression.Bind(_successPropertyInfo, _.BoolExpression),
                        Expression.Bind(_codesPropertyInfo, Expression.Constant(_.Code))
                    ), _.TypeExpression))
                .Map(_ => Expression.Lambda<Func<T, RuleApplicationResult>>(_.Item1, _.TypeExpression))
                .Map(_ => _.Compile());

        private Option<ExpressionTypeCodeBinding> CompileDirectRule<T>(Rule rule) =>
            Try(() =>
                {
                    var genericType = Expression.Parameter(typeof(T));
                    var propertyType = GetTypeFromPropertyName<T>(rule.Property);

                    var value = Expression.Constant(propertyType.BaseType == typeof(Enum)
                        ? Enum.Parse(propertyType, rule.Value)
                        : Convert.ChangeType(rule.Value, propertyType));

                    return new ExpressionTypeCodeBinding
                    {
                        BoolExpression = Expression.MakeBinary(OperationMappings.DirectMapping[rule.Operator],
                            Expression.Property(genericType, rule.Property), value),
                        TypeExpression = genericType,
                        Code = rule.Code
                    }.ToOption();
                })
                .Match(_ => _, e =>
                {
                    _logger.Error(e, "{Component} raised an exception with {Message} when compiling {Rule}", Component,
                        e.Message, rule);
                    return Option<ExpressionTypeCodeBinding>.None();
                });

        private Option<ExpressionTypeCodeBinding> CompileInternalDirectRule<T>(Rule rule)
        {
            const string method = nameof(CompileInternalDirectRule);

            return Try(() =>
                {
                    var genericType = Expression.Parameter(typeof(T));
                    var key = Expression.Property(genericType, rule.Property);
                    var propertyType = GetTypeFromPropertyName<T>(rule.Property);
                    var type1 = propertyType.FullName;

                    var key2 = Expression.Property(genericType, rule.Value);
                    var propertyType2 = GetTypeFromPropertyName<T>(rule.Value);
                    var type2 = propertyType2.FullName;

                    if (type1 != type2)
                    {
                        _logger.Error(
                            "{Component} {Operation}: {Property1} is of type {Type1} while {Property2} is of type {Type2}, no direct comparison possible",
                            Component, method, propertyType, type1, propertyType2, type2);
                        return Option<ExpressionTypeCodeBinding>.None();
                    }

                    return new ExpressionTypeCodeBinding
                    {
                        BoolExpression = Expression.MakeBinary(OperationMappings.InternalDirectMapping[rule.Operator],
                            key, key2),
                        TypeExpression = genericType,
                        Code = rule.Code
                    }.ToOption();
                })
                .Match(_ => _, e =>
                {
                    _logger.Error(e, "{Component} raised an exception with {Message} when compiling {Rule}", Component,
                        e.Message, rule);
                    return Option<ExpressionTypeCodeBinding>.None();
                });
        }

        private Option<ExpressionTypeCodeBinding> CompileEnumerableRule<T>(Rule rule) =>
            Try(() =>
                {
                    var genericType = Expression.Parameter(typeof(T));
                    var key = Expression.Property(genericType, rule.Property);
                    var propertyType = GetTypeFromPropertyName<T>(rule.Property);
                    var searchValuesType = propertyType.IsArray
                        ? propertyType.GetElementType()
                        : propertyType.GetGenericArguments().FirstOrDefault();

                    return new ExpressionTypeCodeBinding
                    {
                        BoolExpression =
                            OperationMappings.EnumerableMapping[rule.Operator](rule, key, searchValuesType),
                        TypeExpression = genericType,
                        Code = rule.Code
                    }.ToOption();
                })
                .Match(_ => _, e =>
                {
                    _logger.Error(e, "{Component} raised an exception with {Message} when compiling {Rule}", Component,
                        e.Message, rule);
                    return Option<ExpressionTypeCodeBinding>.None();
                });

        private static Type GetTypeFromPropertyName<T>(string name)
            => typeof(T)
                .GetProperty(name)
                .PropertyType;

        private Option<ExpressionTypeCodeBinding> CompileInternalEnumerableRule<T>(Rule rule)
        {
            const string method = nameof(CompileInternalEnumerableRule);
            return Try(() =>
                {
                    var genericType = Expression.Parameter(typeof(T));

                    var key = Expression.Property(genericType, rule.Property);
                    var propertyType = GetTypeFromPropertyName<T>(rule.Property);
                    var searchValueType = propertyType.IsArray
                        ? propertyType.GetElementType()
                        : propertyType.GetGenericArguments().FirstOrDefault();
                    var key2 = Expression.Property(genericType, rule.Value);
                    var propertyType2 = GetTypeFromPropertyName<T>(rule.Value);

                    if (searchValueType.FullName != propertyType2.FullName)
                    {
                        _logger.Error(
                            "{Component} {Operation}: {Property1} is of type IEnumerable[{Type1}] while {Property2} is of type {Type2}, no comparison possible",
                            Component, method, propertyType, searchValueType.FullName, propertyType2,
                            propertyType2.FullName);
                        return Option<ExpressionTypeCodeBinding>.None();
                    }

                    return new ExpressionTypeCodeBinding
                    {
                        BoolExpression = OperationMappings.InternalEnumerableMapping[rule.Operator](rule, key,
                            propertyType, key2, propertyType2, searchValueType),
                        TypeExpression = genericType,
                        Code = rule.Code
                    }.ToOption();
                })
                .Match(_ => _, e =>
                {
                    _logger.Error(e, "{Component} raised an exception with {Message} when compiling {Rule}", Component,
                        e.Message, rule);
                    return Option<ExpressionTypeCodeBinding>.None();
                });
        }

        private Option<ExpressionTypeCodeBinding> CompileInternalCrossEnumerableRule<T>(Rule rule)
        {
            const string method = nameof(CompileInternalCrossEnumerableRule);

            return Try(() =>
                {
                    var genericType = Expression.Parameter(typeof(T));

                    var key = Expression.Property(genericType, rule.Property);
                    var propertyType = GetTypeFromPropertyName<T>(rule.Property);
                    var searchValueType = propertyType.IsArray
                        ? propertyType.GetElementType()
                        : propertyType.GetGenericArguments().FirstOrDefault();

                    var key2 = Expression.Property(genericType, rule.Value);
                    var propertyType2 = GetTypeFromPropertyName<T>(rule.Value);

                    if (propertyType != propertyType2)
                    {
                        _logger.Error(
                            "{Component} {Operation}: {Property1} is of type {PropertyType1} while {Property2} is of type {PropertyType2}, no comparison possible",
                            Component, method, propertyType, propertyType, propertyType2, propertyType2.FullName);
                        return Option<ExpressionTypeCodeBinding>.None();
                    }

                    return Some(new ExpressionTypeCodeBinding
                    {
                        BoolExpression = OperationMappings.InternalCrossEnumerableMapping[rule.Operator](rule, key,
                            propertyType, key2, propertyType2, searchValueType),
                        TypeExpression = genericType,
                        Code = rule.Code
                    });
                })
                .Match(_ => _, e =>
                {
                    _logger.Error(e, "{Component} raised an exception with {Message} when compiling {Rule}", Component,
                        e.Message, rule);
                    return Option<ExpressionTypeCodeBinding>.None();
                });

            //try
            //{
            //    var genericType = Expression.Parameter(typeof(T));

            //    var key = Expression.Property(genericType, rule.Property);
            //    var propertyType = GetTypeFromPropertyName<T>(rule.Property);
            //    var searchValueType = propertyType.IsArray ? propertyType.GetElementType() : propertyType.GetGenericArguments().FirstOrDefault();

            //    var key2 = Expression.Property(genericType, rule.Value);
            //    var propertyType2 = GetTypeFromPropertyName<T>(rule.Value);

            //    if (propertyType != propertyType2)
            //    {
            //        _logger.Error(
            //            "{Component} {Operation}: {Property1} is of type {PropertyType1} while {Property2} is of type {PropertyType2}, no comparison possible",
            //            Component, method, propertyType, propertyType, propertyType2, propertyType2.FullName);
            //        return null;
            //    }
                
            //    return new ExpressionTypeCodeBinding
            //    {
            //        BoolExpression = OperationMappings.InternalCrossEnumerableMapping[rule.Operator](rule, key, propertyType, key2, propertyType2, searchValueType),
            //        TypeExpression = genericType,
            //        Code = rule.Code
            //    };
            //}
            //catch (Exception e)
            //{
            //    _logger.Error(e, "{Component} raised an exception with {Message} when compiling {Rule}", Component,
            //        e.Message, rule);
            //    return null;
            //}
        }

        private Option<ExpressionTypeCodeBinding> CompileExternalEnumerableRule<T>(Rule rule) =>
            Try(() =>
                {
                    var genericType = Expression.Parameter(typeof(T));
                    var propertyType = GetTypeFromPropertyName<T>(rule.Property);

                    return new ExpressionTypeCodeBinding
                    {
                        BoolExpression = OperationMappings.ExternalEnumerableMapping[rule.Operator](
                            Expression.Property(genericType, rule.Property),
                            propertyType, Expression.NewArrayInit(propertyType, rule.Value.Split(',')
                                .Select(v => Convert.ChangeType(v, propertyType, CultureInfo.InvariantCulture))
                                .Select(Expression.Constant))),
                        TypeExpression = genericType,
                        Code = rule.Code
                    }.ToOption();
                })
                .Match(_ => _, e =>
                {
                    _logger.Error(e, "{Component} raised an exception with {Message} when compiling {Rule}", Component,
                        e.Message, rule);
                    return Option<ExpressionTypeCodeBinding>.None();
                });

        private Option<ExpressionTypeCodeBinding> CompileExternalKeyValueRule<T>(Rule rule) =>
            Try(() =>
                {
                    var type = typeof(T);
                    var genericType = Expression.Parameter(type);

                    return Some(new ExpressionTypeCodeBinding
                    {
                        BoolExpression =
                            OperationMappings.ExternalKeyValueMapping[rule.Operator](genericType, rule, type),
                        TypeExpression = genericType,
                        Code = rule.Code
                    });
                })
                .Match(_ => _, e =>
                {
                    _logger.Error(e, "{Component} raised an exception with {Message} when compiling {Rule}", Component,
                        e.Message, rule);
                    return Option<ExpressionTypeCodeBinding>.None();
                });

        private Option<ExpressionTypeCodeBinding> CreateCompiledRule<T>(Rule rule) =>
            OperatorClassification.GetOperatorType(rule.Operator) switch
            {
                OperatorCategory.Direct => CompileDirectRule<T>(rule),
                OperatorCategory.Enumerable => CompileEnumerableRule<T>(rule),
                OperatorCategory.InternalDirect => CompileInternalDirectRule<T>(rule),
                OperatorCategory.InternalEnumerable => CompileInternalEnumerableRule<T>(rule),
                OperatorCategory.InternalCrossEnumerable => CompileInternalCrossEnumerableRule<T>(rule),
                OperatorCategory.ExternalEnumerable => CompileExternalEnumerableRule<T>(rule),
                OperatorCategory.KeyValue => CompileExternalKeyValueRule<T>(rule),
                _ => Option<ExpressionTypeCodeBinding>.None()
            };
    }

}