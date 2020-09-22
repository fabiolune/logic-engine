using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Fabiolune.BusinessRulesEngine.Interfaces;
using Fabiolune.BusinessRulesEngine.Internals;
using Fabiolune.BusinessRulesEngine.Models;
using Serilog;

namespace Fabiolune.BusinessRulesEngine
{
    public class BusinessRulesCompiler : IBusinessRulesCompiler
    {
        private const string Component = nameof(BusinessRulesCompiler);
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
                };
            }
            catch (Exception e)
            {
                _logger.Error(e, "{Component} raised an exception with {Message} when compiling {Rule}", Component,
                    e.Message, rule);
                return null;
            }
        }

        private ExpressionTypeCodeBinding CompileInternalDirectRule<T>(Rule rule)
        {
            const string method = nameof(CompileInternalDirectRule);

            try
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
                    return null;
                }

                return new ExpressionTypeCodeBinding
                {
                    BoolExpression = Expression.MakeBinary(OperationMappings.InternalDirectMapping[rule.Operator], key, key2),
                    TypeExpression = genericType,
                    Code = rule.Code
                };
            }
            catch (Exception e)
            {
                _logger.Error(e, "{Component} raised an exception with {Message} when compiling {Rule}", Component,
                    e.Message, rule);
                return null;
            }
        }

        private ExpressionTypeCodeBinding CompileEnumerableRule<T>(Rule rule)
        {
            try
            {
                var genericType = Expression.Parameter(typeof(T));
                var key = Expression.Property(genericType, rule.Property);
                var propertyType = GetTypeFromPropertyName<T>(rule.Property);
                var searchValuesType = propertyType.IsArray
                    ? propertyType.GetElementType()
                    : propertyType.GetGenericArguments().FirstOrDefault();

                return new ExpressionTypeCodeBinding
                {
                    BoolExpression = OperationMappings.EnumerableMapping[rule.Operator](rule, key, searchValuesType),
                    TypeExpression = genericType,
                    Code = rule.Code
                };
            }
            catch (Exception e)
            {
                _logger.Error(e, "{Component} raised an exception with {Message} when compiling {Rule}", Component,
                    e.Message, rule);
                return null;
            }
        }

        private static Type GetTypeFromPropertyName<T>(string name)
            => typeof(T).GetProperty(name).PropertyType;

        private ExpressionTypeCodeBinding CompileInternalEnumerableRule<T>(Rule rule)
        {
            const string method = nameof(CompileInternalEnumerableRule);
            try
            {
                var genericType = Expression.Parameter(typeof(T));

                var key = Expression.Property(genericType, rule.Property);
                var propertyType = GetTypeFromPropertyName<T>(rule.Property);
                var searchValueType = propertyType.IsArray ? propertyType.GetElementType() : propertyType.GetGenericArguments().FirstOrDefault();
                var key2 = Expression.Property(genericType, rule.Value);
                var propertyType2 = GetTypeFromPropertyName<T>(rule.Value);

                if (searchValueType.FullName != propertyType2.FullName)
                {
                    _logger.Error(
                        "{Component} {Operation}: {Property1} is of type IEnumerable[{Type1}] while {Property2} is of type {Type2}, no comparison possible",
                        Component, method, propertyType, searchValueType.FullName, propertyType2, propertyType2.FullName);
                    return null;
                }

                return new ExpressionTypeCodeBinding
                {
                    BoolExpression = OperationMappings.InternalEnumerableMapping[rule.Operator](rule, key, propertyType, key2, propertyType2, searchValueType),
                    TypeExpression = genericType,
                    Code = rule.Code
                };
            }
            catch (Exception e)
            {
                _logger.Error(e, "{Component} raised an exception with {Message} when compiling {Rule}", Component,
                    e.Message, rule);
                return null;
            }
        }

        private ExpressionTypeCodeBinding CompileInternalCrossEnumerableRule<T>(Rule rule)
        {
            const string method = nameof(CompileInternalCrossEnumerableRule);
            try
            {
                var genericType = Expression.Parameter(typeof(T));

                var key = Expression.Property(genericType, rule.Property);
                var propertyType = GetTypeFromPropertyName<T>(rule.Property);
                var searchValueType = propertyType.IsArray ? propertyType.GetElementType() : propertyType.GetGenericArguments().FirstOrDefault();

                var key2 = Expression.Property(genericType, rule.Value);
                var propertyType2 = GetTypeFromPropertyName<T>(rule.Value);

                if (propertyType != propertyType2)
                {
                    _logger.Error(
                        "{Component} {Operation}: {Property1} is of type {PropertyType1} while {Property2} is of type {PropertyType2}, no comparison possible",
                        Component, method, propertyType, propertyType, propertyType2, propertyType2.FullName);
                    return null;
                }
                
                return new ExpressionTypeCodeBinding
                {
                    BoolExpression = OperationMappings.InternalCrossEnumerableMapping[rule.Operator](rule, key, propertyType, key2, propertyType2, searchValueType),
                    TypeExpression = genericType,
                    Code = rule.Code
                };
            }
            catch (Exception e)
            {
                _logger.Error(e, "{Component} raised an exception with {Message} when compiling {Rule}", Component,
                    e.Message, rule);
                return null;
            }
        }

        private ExpressionTypeCodeBinding CompileExternalEnumerableRule<T>(Rule rule)
        {
            try
            {
                var genericType = Expression.Parameter(typeof(T));
                var propertyType = GetTypeFromPropertyName<T>(rule.Property);

                return new ExpressionTypeCodeBinding
                {
                    BoolExpression = OperationMappings.ExternalEnumerableMapping[rule.Operator](Expression.Property(genericType, rule.Property),
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
                    e.Message, rule);
                return null;
            }
        }

        private ExpressionTypeCodeBinding CompileExternalKeyValueRule<T>(Rule rule)
        {
            try
            {
                var type = typeof(T);
                var genericType = Expression.Parameter(type);

                return new ExpressionTypeCodeBinding
                {
                    BoolExpression = OperationMappings.ExternalKeyValueMapping[rule.Operator](genericType, rule, type),
                    TypeExpression = genericType,
                    Code = rule.Code
                };
            }
            catch (Exception e)
            {
                _logger.Error(e, "{Component} raised an exception with {Message} when compiling {Rule}", Component,
                    e.Message, rule);
                return null;
            }
        }

        private ExpressionTypeCodeBinding CreateCompiledRule<T>(Rule rule)
        {
            switch (OperatorClassification.GetOperatorType(rule.Operator))
            {
                case OperatorCategory.Direct:
                    return CompileDirectRule<T>(rule);
                case OperatorCategory.Enumerable:
                    return CompileEnumerableRule<T>(rule);
                case OperatorCategory.InternalDirect:
                    return CompileInternalDirectRule<T>(rule);
                case OperatorCategory.InternalEnumerable:
                    return CompileInternalEnumerableRule<T>(rule);
                case OperatorCategory.InternalCrossEnumerable:
                    return CompileInternalCrossEnumerableRule<T>(rule);
                case OperatorCategory.ExternalEnumerable:
                    return CompileExternalEnumerableRule<T>(rule);
                case OperatorCategory.KeyValue:
                    return CompileExternalKeyValueRule<T>(rule);
                default:
                    return null;
            }
        }
    }

}