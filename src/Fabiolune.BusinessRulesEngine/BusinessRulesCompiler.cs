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
            const string successFieldName = nameof(RuleApplicationResult.Success);
            const string codeFieldName = nameof(RuleApplicationResult.Code);
            _successPropertyInfo = ResultType.GetProperty(successFieldName);
            _codesPropertyInfo = ResultType.GetProperty(codeFieldName);
        }

        public IEnumerable<Func<T, RuleApplicationResult>> CompileRules<T>(IEnumerable<Rule> rules)
        {
            return rules
                .Select(r => GenerateFunc<T>(CreateCompiledRule<T>(r)))
                .Where(r => r != null);
        }

        private Func<T, RuleApplicationResult> GenerateFunc<T>(ExpressionTypeCodeBinding pair)
        {
            if (pair?.BoolExpression == null)
                return null;

            var resultExpression =
                string.IsNullOrEmpty(pair.Code)
                    ? Expression.MemberInit(
                        Expression.New(ResultType),
                        Expression.Bind(_successPropertyInfo, pair.BoolExpression))
                    : Expression.MemberInit(
                        Expression.New(ResultType),
                        Expression.Bind(_successPropertyInfo, pair.BoolExpression),
                        Expression.Bind(_codesPropertyInfo, Expression.Constant(pair.Code))
                    );

            return Expression.Lambda<Func<T, RuleApplicationResult>>(resultExpression, pair.TypeExpression).Compile();
        }

        private ExpressionTypeCodeBinding CompileDirectRule<T>(Rule rule)
        {
            const string method = nameof(CompileDirectRule);

            try
            {
                ExpressionType expressionType;
                switch (rule.Operator)
                {
                    case OperatorType.Equal:
                        expressionType = ExpressionType.Equal;
                        break;
                    case OperatorType.GreaterThan:
                        expressionType = ExpressionType.GreaterThan;
                        break;
                    case OperatorType.GreaterThanOrEqual:
                        expressionType = ExpressionType.GreaterThanOrEqual;
                        break;
                    case OperatorType.LessThan:
                        expressionType = ExpressionType.LessThan;
                        break;
                    case OperatorType.LessThanOrEqual:
                        expressionType = ExpressionType.LessThanOrEqual;
                        break;
                    case OperatorType.NotEqual:
                        expressionType = ExpressionType.NotEqual;
                        break;
                    default:
                        _logger.Warning("{Component} {Operation} unable to compile {Rule}", Component, method, rule);
                        return null;
                }

                var genericType = Expression.Parameter(typeof(T));
                var key = Expression.Property(genericType, rule.Property);
                var propertyType = typeof(T).GetProperty(rule.Property).PropertyType;

                var value = Expression.Constant(propertyType.BaseType == typeof(Enum)
                        ? Enum.Parse(propertyType, rule.Value)
                        : Convert.ChangeType(rule.Value, propertyType));

                var boolExpression = Expression.MakeBinary(expressionType, key, value);
                return new ExpressionTypeCodeBinding {BoolExpression = boolExpression, TypeExpression = genericType, Code = rule.Code};
            }
            catch (Exception e)
            {
                _logger.Error(e, "{Component} raised an exception with {Message} when compiling {Rule}", Component, e.Message, JsonConvert.SerializeObject(rule, Formatting.Indented));
                return null;
            }
        }

        private ExpressionTypeCodeBinding CompileInternalDirectRule<T>(Rule rule)
        {
            const string method = nameof(CompileInternalDirectRule);
            try
            {
                ExpressionType expressionType;
                switch (rule.Operator)
                {
                    case OperatorType.InnerEqual:
                        expressionType = ExpressionType.Equal;
                        break;
                    case OperatorType.InnerGreaterThan:
                        expressionType = ExpressionType.GreaterThan;
                        break;
                    case OperatorType.InnerGreaterThanOrEqual:
                        expressionType = ExpressionType.GreaterThanOrEqual;
                        break;
                    case OperatorType.InnerLessThan:
                        expressionType = ExpressionType.LessThan;
                        break;
                    case OperatorType.InnerLessThanOrEqual:
                        expressionType = ExpressionType.LessThanOrEqual;
                        break;
                    case OperatorType.InnerNotEqual:
                        expressionType = ExpressionType.NotEqual;
                        break;
                    default:
                        _logger.Warning("{Component} {Operation} unable to compile {Rule}", Component, method, rule);
                        return null;
                }

                var genericType = Expression.Parameter(typeof(T));
                var key = Expression.Property(genericType, rule.Property);
                var propertyType = typeof(T).GetProperty(rule.Property).PropertyType;
                var type1 = propertyType.FullName;

                var key2 = Expression.Property(genericType, rule.Value);
                var propertyType2 = typeof(T).GetProperty(rule.Value).PropertyType;
                var type2 = propertyType2.FullName;

                if (type1 != type2)
                {
                    _logger.Error("{Component} {Operation}: {Property1} is of type {Type1} while {Property2} is of type {Type2}, no direct comparison possible", Component, method, propertyType, type1, propertyType2, type2);
                    return null;
                }

                var boolExpression = Expression.MakeBinary(expressionType, key, key2);
                return new ExpressionTypeCodeBinding {BoolExpression = boolExpression, TypeExpression = genericType, Code = rule.Code};
            }
            catch (Exception e)
            {
                _logger.Error(e, "{Component} raised an exception with {Message} when compiling {Rule}", Component, e.Message, JsonConvert.SerializeObject(rule, Formatting.Indented));
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
                
                var searchValuesType = propertyType.IsArray ? propertyType.GetElementType() : propertyType.GetGenericArguments().FirstOrDefault();

                ConstantExpression key2;
                BinaryExpression boolExpression;
                Expression leftBody;
                Expression rightBody;
                IEnumerable<ConstantExpression> array;
                NewArrayExpression arrayExpression;

                switch (rule.Operator)
                {
                    case OperatorType.Contains:
                        key2 = Expression.Constant(Convert.ChangeType(rule.Value, searchValuesType));
                        leftBody = Expression.MakeBinary(ExpressionType.NotEqual, key, NullValue);
                        rightBody = Expression.Call(typeof(Enumerable), nameof(Enumerable.Contains), new[] {searchValuesType}, key, key2);
                        boolExpression = Expression.MakeBinary(ExpressionType.AndAlso, leftBody, rightBody);
                        break;
                    case OperatorType.NotContains:
                        key2 = Expression.Constant(Convert.ChangeType(rule.Value, searchValuesType));
                        leftBody = Expression.MakeBinary(ExpressionType.Equal, key, NullValue);
                        rightBody = Expression.IsFalse(Expression.Call(typeof(Enumerable), nameof(Enumerable.Contains), new[] {searchValuesType}, key, key2));
                        boolExpression = Expression.MakeBinary(ExpressionType.OrElse, leftBody, rightBody);
                        break;
                    case OperatorType.Overlaps:
                        array = rule.Value.Split(',')
                            .Select(v => Convert.ChangeType(v, searchValuesType, CultureInfo.InvariantCulture))
                            .Select(Expression.Constant);
                        arrayExpression = Expression.NewArrayInit(searchValuesType, array);

                        leftBody = Expression.MakeBinary(ExpressionType.AndAlso,
                            Expression.MakeBinary(ExpressionType.NotEqual, key, NullValue),
                            Expression.MakeBinary(ExpressionType.NotEqual, arrayExpression, NullValue));

                        rightBody = Expression.IsTrue(
                            Expression.Call(
                                typeof(Enumerable), "Any",
                                new[] {searchValuesType},
                                Expression.Call(typeof(Enumerable), "Intersect", new[] {searchValuesType}, key, arrayExpression)
                            )
                        );

                        boolExpression = Expression.MakeBinary(ExpressionType.AndAlso, leftBody, rightBody);
                        break;
                    case OperatorType.NotOverlaps:
                        array = rule.Value.Split(',')
                            .Select(v => Convert.ChangeType(v, searchValuesType, CultureInfo.InvariantCulture))
                            .Select(Expression.Constant);
                        arrayExpression = Expression.NewArrayInit(searchValuesType, array);

                        leftBody = Expression.MakeBinary(ExpressionType.OrElse,
                            Expression.MakeBinary(ExpressionType.Equal, key, NullValue),
                            Expression.MakeBinary(ExpressionType.Equal, arrayExpression, NullValue));

                        rightBody = Expression.IsFalse(
                            Expression.Call(
                                typeof(Enumerable), "Any",
                                new[] {searchValuesType},
                                Expression.Call(typeof(Enumerable), "Intersect", new[] {searchValuesType}, key, arrayExpression)
                            )
                        );

                        boolExpression = Expression.MakeBinary(ExpressionType.OrElse, leftBody, rightBody);
                        break;
                    default:
                        _logger.Error("{Component} Invalid operator in {Rule}", Component, JsonConvert.SerializeObject(rule, Formatting.Indented));
                        return null;
                }

                return new ExpressionTypeCodeBinding {BoolExpression = boolExpression, TypeExpression = genericType, Code = rule.Code};
            }
            catch (Exception e)
            {
                _logger.Error(e, "{Component} raised an exception with {Message} when compiling {Rule}", Component, e.Message, JsonConvert.SerializeObject(rule, Formatting.Indented));
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
                var searchValuesType = propertyType.IsArray ? propertyType.GetElementType() : propertyType.GetGenericArguments().FirstOrDefault();
                var type1 = searchValuesType.FullName;

                var key2 = Expression.Property(genericType, rule.Value);
                var propertyType2 = typeof(T).GetProperty(rule.Value).PropertyType;
                var type2 = propertyType2.FullName;

                BinaryExpression boolExpression;
                Expression leftBody;
                Expression rightBody;

                switch (rule.Operator)
                {
                    case OperatorType.InnerContains:
                        if (type1 != type2)
                        {
                            _logger.Error("{Component} {Operation}: {Property1} is of type IENumerable[{Type1}] while {Property2} is of type {Type2}, no comparison possible", Component, method, propertyType, type1, propertyType2, type2);
                            return null;
                        }

                        leftBody = Expression.MakeBinary(ExpressionType.NotEqual, key, NullValue);
                        rightBody = Expression.Call(typeof(Enumerable), nameof(Enumerable.Contains), new[] {searchValuesType}, key, key2);
                        boolExpression = Expression.MakeBinary(ExpressionType.AndAlso, leftBody, rightBody);
                        break;
                    case OperatorType.InnerNotContains:
                        if (type1 != type2)
                        {
                            _logger.Error("{Component} {Operation}: {Property1} is of type IENumerable[{Type1}] while {Property2} is of type {Type2}, no comparison possible", Component, method, propertyType, type1, propertyType2, type2);
                            return null;
                        }

                        leftBody = Expression.MakeBinary(ExpressionType.Equal, key, NullValue);
                        rightBody = Expression.IsFalse(Expression.Call(typeof(Enumerable), nameof(Enumerable.Contains), new[] {searchValuesType}, key, key2));
                        boolExpression = Expression.MakeBinary(ExpressionType.OrElse, leftBody, rightBody);
                        break;
                    case OperatorType.InnerOverlaps:
                        if (propertyType != propertyType2)
                        {
                            _logger.Error("{Component} {Operation}: {Property1} is of type {PropertyType1} while {Property2} is of type {PropertyType2}, no comparison possible", Component, method, propertyType, propertyType, propertyType2, type2);
                            return null;
                        }

                        // both are not null
                        leftBody = Expression.MakeBinary(
                            ExpressionType.AndAlso,
                            Expression.MakeBinary(ExpressionType.NotEqual, key, NullValue),
                            Expression.MakeBinary(ExpressionType.NotEqual, key2, NullValue)
                        );

                        // intersection is not empty
                        rightBody = Expression.IsTrue(Expression.Call(
                            typeof(Enumerable),
                            "Any",
                            new[] {searchValuesType},
                            Expression.Call(typeof(Enumerable), "Intersect", new[] {searchValuesType}, key, key2)
                        ));

                        boolExpression = Expression.MakeBinary(ExpressionType.AndAlso, leftBody, rightBody);
                        break;
                    case OperatorType.InnerNotOverlaps:
                        if (propertyType != propertyType2)
                        {
                            _logger.Error("{Component} {Operation}: {Property1} is of type {PropertyType1} while {Property2} is of type {PropertyType2}, no comparison possible", Component, method, propertyType, propertyType, propertyType2, type2);
                            return null;
                        }

                        // one of them is null
                        leftBody = Expression.MakeBinary(
                            ExpressionType.OrElse,
                            Expression.MakeBinary(ExpressionType.Equal, key, NullValue),
                            Expression.MakeBinary(ExpressionType.Equal, key2, NullValue)
                        );
                        
                        // intersection is empty
                        rightBody = Expression.IsFalse(Expression.Call(
                            typeof(Enumerable),
                            nameof(Enumerable.Any),
                            new[] {searchValuesType},
                            Expression.Call(typeof(Enumerable), "Intersect", new[] {searchValuesType}, key, key2)
                        ));

                        boolExpression = Expression.MakeBinary(ExpressionType.OrElse, leftBody, rightBody);
                        break;
                    default:
                        _logger.Error("{Component} Invalid operator in {Rule}", Component, JsonConvert.SerializeObject(rule, Formatting.Indented));
                        return null;
                }

                return new ExpressionTypeCodeBinding {BoolExpression = boolExpression, TypeExpression = genericType, Code = rule.Code};
            }
            catch (Exception e)
            {
                _logger.Error(e, "{Component} raised an exception with {Message} when compiling {Rule}", Component, e.Message, JsonConvert.SerializeObject(rule, Formatting.Indented));
                return null;
            }
        }

        private ExpressionTypeCodeBinding CompileExternalEnumerableRule<T>(Rule rule)
        {
            try
            {
                var genericType = Expression.Parameter(typeof(T));

                var key = Expression.Property(genericType, rule.Property);
                var propertyType = typeof(T).GetProperty(rule.Property).PropertyType;

                var array = rule.Value.Split(',')
                    .Select(v => Convert.ChangeType(v, propertyType, CultureInfo.InvariantCulture))
                    .Select(Expression.Constant);
                var arrayExpression = Expression.NewArrayInit(propertyType, array);

                BinaryExpression boolExpression;
                Expression leftBody;
                Expression rightBody;
                
                switch (rule.Operator)
                {
                    case OperatorType.IsContained:
                        leftBody = Expression.MakeBinary(ExpressionType.NotEqual, arrayExpression, NullValue);
                        rightBody = Expression.Call(typeof(Enumerable), nameof(Enumerable.Contains), new[] {propertyType}, arrayExpression, key);
                        boolExpression = Expression.MakeBinary(ExpressionType.AndAlso, leftBody, rightBody);
                        break;
                    case OperatorType.IsNotContained:
                        leftBody = Expression.MakeBinary(ExpressionType.Equal, arrayExpression, NullValue);
                        rightBody = Expression.IsFalse(Expression.Call(typeof(Enumerable), nameof(Enumerable.Contains), new[] {propertyType}, arrayExpression, key));
                        boolExpression = Expression.MakeBinary(ExpressionType.OrElse, leftBody, rightBody);
                        break;
                    default:
                        _logger.Error("{Component} Invalid operator in {Rule}", Component, JsonConvert.SerializeObject(rule, Formatting.Indented));
                        return null;
                }

                return new ExpressionTypeCodeBinding {BoolExpression = boolExpression, TypeExpression = genericType, Code = rule.Code};
            }
            catch (Exception e)
            {
                _logger.Error(e, "{Component} raised an exception with {Message} when compiling {Rule}", Component, e.Message, JsonConvert.SerializeObject(rule, Formatting.Indented));
                return null;
            }
        }

        private ExpressionTypeCodeBinding CompileExternalKeyValueRule<T>(Rule rule)
        {
            try
            {
                var type = typeof(T);
                var genericType = Expression.Parameter(typeof(T));

                MemberExpression property;
                Type propertyType;
                Type[] typeParameters;
                Type keyType;
                Type valueType;
                string[] parts;

                var boolExpression = default(BinaryExpression);
                Expression leftBody;
                Expression rightBody;
                var getItemMethodInfo = typeof(Dictionary<string, string>).GetMethod("get_Item");
                var containsKeyMethodInfo = typeof(IDictionary<string, string>).GetMethod("ContainsKey");
                var containsValueMethodInfo = typeof(Dictionary<string, string>).GetMethod("ContainsValue");

                switch (rule.Operator)
                {
                    case OperatorType.ContainsKey:
                        property = Expression.Property(genericType, rule.Property);
                        propertyType = type.GetProperty(rule.Property).PropertyType;
                        typeParameters = propertyType.GetGenericArguments();
                        keyType = typeParameters[0];
                        leftBody = Expression.MakeBinary(ExpressionType.NotEqual, property, NullValue);
                        rightBody = Expression.Call(property, containsKeyMethodInfo, Expression.Constant(Convert.ChangeType(rule.Value, keyType)));
                        boolExpression = Expression.MakeBinary(ExpressionType.AndAlso, leftBody, rightBody);
                        break;
                    case OperatorType.NotContainsKey:
                        property = Expression.Property(genericType, rule.Property);
                        propertyType = type.GetProperty(rule.Property).PropertyType;
                        typeParameters = propertyType.GetGenericArguments();
                        keyType = typeParameters[0];
                        leftBody = Expression.MakeBinary(ExpressionType.Equal, property, NullValue);
                        rightBody = Expression.IsFalse(Expression.Call(property, containsKeyMethodInfo, Expression.Constant(Convert.ChangeType(rule.Value, keyType))));
                        boolExpression = Expression.MakeBinary(ExpressionType.OrElse, leftBody, rightBody);
                        break;
                    case OperatorType.ContainsValue:
                        property = Expression.Property(genericType, rule.Property);
                        propertyType = type.GetProperty(rule.Property).PropertyType;
                        typeParameters = propertyType.GetGenericArguments();
                        valueType = typeParameters[1];
                        leftBody = Expression.MakeBinary(ExpressionType.NotEqual, property, NullValue);
                        rightBody = Expression.Call(property, containsValueMethodInfo, Expression.Constant(Convert.ChangeType(rule.Value, valueType)));
                        boolExpression = Expression.MakeBinary(ExpressionType.AndAlso, leftBody, rightBody);
                        break;
                    case OperatorType.NotContainsValue:
                        property = Expression.Property(genericType, rule.Property);
                        propertyType = type.GetProperty(rule.Property).PropertyType;
                        typeParameters = propertyType.GetGenericArguments();
                        keyType = typeParameters[0];
                        leftBody = Expression.MakeBinary(ExpressionType.Equal, property, NullValue);
                        rightBody = Expression.IsFalse(Expression.Call(property, containsValueMethodInfo, Expression.Constant(Convert.ChangeType(rule.Value, keyType))));
                        boolExpression = Expression.MakeBinary(ExpressionType.OrElse, leftBody, rightBody);
                        break;
                    case OperatorType.KeyContainsValue:
                        parts = rule.Property.Split("[".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(_ => _.TrimEnd(']')).ToArray();
                        property = Expression.Property(genericType, parts[0]);
                        propertyType = type.GetProperty(parts[0]).PropertyType;
                        typeParameters = propertyType.GetGenericArguments();
                        keyType = typeParameters[0];
                        valueType = typeParameters[1];
                        leftBody = Expression.MakeBinary(ExpressionType.NotEqual, property, NullValue);
                        leftBody = Expression.MakeBinary(ExpressionType.AndAlso, leftBody, Expression.Call(property, containsKeyMethodInfo, Expression.Constant(Convert.ChangeType(parts[1], keyType))));
                        var exp = Expression.Call(property, getItemMethodInfo, Expression.Constant(Convert.ChangeType(parts[1], keyType)));
                        rightBody = Expression.MakeBinary(ExpressionType.Equal, exp, Expression.Constant(Convert.ChangeType(rule.Value, valueType)));
                        boolExpression = Expression.MakeBinary(ExpressionType.AndAlso, leftBody, rightBody);
                        break;
                    case OperatorType.NotKeyContainsValue:
                        parts = rule.Property.Split("[".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(_ => _.TrimEnd(']')).ToArray();
                        property = Expression.Property(genericType, parts[0]);
                        propertyType = type.GetProperty(parts[0]).PropertyType;
                        typeParameters = propertyType.GetGenericArguments();
                        keyType = typeParameters[0];
                        valueType = typeParameters[1];
                        leftBody = Expression.MakeBinary(ExpressionType.Equal, property, NullValue);
                        leftBody = Expression.MakeBinary(ExpressionType.OrElse, leftBody, Expression.IsFalse(Expression.Call(property, containsKeyMethodInfo, Expression.Constant(Convert.ChangeType(parts[1], keyType)))));
                        var exp1 = Expression.Call(property, getItemMethodInfo, Expression.Constant(Convert.ChangeType(parts[1], keyType)));
                        rightBody = Expression.IsFalse(Expression.MakeBinary(ExpressionType.Equal, exp1, Expression.Constant(Convert.ChangeType(rule.Value, valueType))));
                        boolExpression = Expression.MakeBinary(ExpressionType.OrElse, leftBody, rightBody);
                        break;
                }

                return new ExpressionTypeCodeBinding {BoolExpression = boolExpression, TypeExpression = genericType, Code = rule.Code};
            }
            catch (Exception e)
            {
                _logger.Error(e, "{Component} raised an exception with {Message} when compiling {Rule}", Component, e.Message, JsonConvert.SerializeObject(rule, Formatting.Indented));
                return null;
            }
        }

        private ExpressionTypeCodeBinding CreateCompiledRule<T>(Rule rule)
        {
            var type = OperatorClassification.GetOperatorType(rule.Operator);
            switch (type)
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