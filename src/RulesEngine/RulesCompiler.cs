using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using RulesEngine.Interfaces;
using RulesEngine.Internals;
using RulesEngine.Models;
using Serilog;
using TinyFp;
using TinyFp.Extensions;
using static System.Linq.Expressions.Expression;
using static TinyFp.Prelude;
using Convert = System.Convert;

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

        public IEnumerable<Func<T, Either<string, Unit>>> CompileRules<T>(IEnumerable<Rule> rules)
            => rules
                .Select(r => GenerateFunc<T>(CreateCompiledRule<T>(r)))
                .Where(r => r.IsSome)
                .Select(f => f.Match(_ => _, () => _ => new RuleApplicationResult()))
                .Select(_ =>
                    new Func<T, Either<string, Unit>>(t => _.Invoke(t).ToOption(x => x.Success)
                        .Match(x => Either<string, Unit>.Left(x.Code ?? string.Empty), () => Unit.Default)));

        private Option<Func<T, RuleApplicationResult>> GenerateFunc<T>(Option<ExpressionTypeCodeBinding> pair) =>
            pair
                .Map(_ => (string.IsNullOrEmpty(_.Code)
                    ? MemberInit(
                        New(ResultType), 
                        Bind(_successPropertyInfo, _.BoolExpression))
                    : MemberInit(
                        New(ResultType),
                        Bind(_successPropertyInfo, _.BoolExpression),
                        Bind(_codesPropertyInfo, Constant(_.Code))
                    ), _.TypeExpression))
                .Map(_ => Lambda<Func<T, RuleApplicationResult>>(_.Item1, _.TypeExpression))
                .Map(_ => _.Compile());

        private Option<ExpressionTypeCodeBinding> CompileDirectRule<T>(Rule rule) =>
            Try(() =>
                {
                    var genericType = Parameter(typeof(T));
                    var propertyType = GetTypeFromPropertyName<T>(rule.Property);

                    var value = Constant(propertyType.BaseType == typeof(Enum)
                        ? Enum.Parse(propertyType, rule.Value)
                        : Convert.ChangeType(rule.Value, propertyType));

                    return Some(new ExpressionTypeCodeBinding
                    {
                        BoolExpression = MakeBinary(OperationMappings.DirectMapping[rule.Operator],
                            Property(genericType, rule.Property), value),
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

        private Option<ExpressionTypeCodeBinding> CompileInternalDirectRule<T>(Rule rule)
        {
            const string method = nameof(CompileInternalDirectRule);

            return Try(() =>
                {
                    var genericType = Parameter(typeof(T));
                    var key = Property(genericType, rule.Property);
                    var propertyType = GetTypeFromPropertyName<T>(rule.Property);
                    var type1 = propertyType.FullName;

                    var key2 = Property(genericType, rule.Value);
                    var propertyType2 = GetTypeFromPropertyName<T>(rule.Value);
                    var type2 = propertyType2.FullName;

                    if (type1 != type2)
                    {
                        _logger.Error(
                            "{Component} {Operation}: {Property1} is of type {Type1} while {Property2} is of type {Type2}, no direct comparison possible",
                            Component, method, propertyType, type1, propertyType2, type2);
                        return Option<ExpressionTypeCodeBinding>.None();
                    }

                    return Some(new ExpressionTypeCodeBinding
                    {
                        BoolExpression = MakeBinary(OperationMappings.InternalDirectMapping[rule.Operator],
                            key, key2),
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
        }

        private Option<ExpressionTypeCodeBinding> CompileEnumerableRule<T>(Rule rule) =>
            Try(() =>
                {
                    var genericType = Parameter(typeof(T));
                    var key = Property(genericType, rule.Property);
                    var propertyType = GetTypeFromPropertyName<T>(rule.Property);
                    var searchValuesType = propertyType.IsArray
                        ? propertyType.GetElementType()
                        : propertyType.GetGenericArguments().FirstOrDefault();

                    return Some(new ExpressionTypeCodeBinding
                    {
                        BoolExpression =
                            OperationMappings.EnumerableMapping[rule.Operator](rule, key, searchValuesType),
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

        private static Type GetTypeFromPropertyName<T>(string name)
            => typeof(T)
                .GetProperty(name)
                .PropertyType;

        private Option<ExpressionTypeCodeBinding> CompileInternalEnumerableRule<T>(Rule rule)
        {
            const string method = nameof(CompileInternalEnumerableRule);
            return Try(() =>
                {
                    var genericType = Parameter(typeof(T));

                    var key = Property(genericType, rule.Property);
                    var propertyType = GetTypeFromPropertyName<T>(rule.Property);
                    var searchValueType = propertyType.IsArray
                        ? propertyType.GetElementType()
                        : propertyType.GetGenericArguments().FirstOrDefault();
                    var key2 = Property(genericType, rule.Value);
                    var propertyType2 = GetTypeFromPropertyName<T>(rule.Value);

                    if (searchValueType.FullName != propertyType2.FullName)
                    {
                        _logger.Error(
                            "{Component} {Operation}: {Property1} is of type IEnumerable[{Type1}] while {Property2} is of type {Type2}, no comparison possible",
                            Component, method, propertyType, searchValueType.FullName, propertyType2,
                            propertyType2.FullName);
                        return Option<ExpressionTypeCodeBinding>.None();
                    }

                    return Some(new ExpressionTypeCodeBinding
                    {
                        BoolExpression = OperationMappings.InternalEnumerableMapping[rule.Operator](rule, key,
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
        }

        private Option<ExpressionTypeCodeBinding> CompileInternalCrossEnumerableRule<T>(Rule rule)
        {
            const string method = nameof(CompileInternalCrossEnumerableRule);

            return Try(() =>
                {
                    var genericType = Parameter(typeof(T));

                    var key = Property(genericType, rule.Property);
                    var propertyType = GetTypeFromPropertyName<T>(rule.Property);
                    var searchValueType = propertyType.IsArray
                        ? propertyType.GetElementType()
                        : propertyType.GetGenericArguments().FirstOrDefault();

                    var key2 = Property(genericType, rule.Value);
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
        }

        private Option<ExpressionTypeCodeBinding> CompileExternalEnumerableRule<T>(Rule rule) =>
            Try(() =>
                {
                    var genericType = Parameter(typeof(T));
                    var propertyType = GetTypeFromPropertyName<T>(rule.Property);

                    return Some(new ExpressionTypeCodeBinding
                    {
                        BoolExpression = OperationMappings.ExternalEnumerableMapping[rule.Operator](
                            Property(genericType, rule.Property),
                            propertyType, NewArrayInit(propertyType, rule.Value.Split(',')
                                .Select(v => Convert.ChangeType(v, propertyType, CultureInfo.InvariantCulture))
                                .Select(Constant))),
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

        private Option<ExpressionTypeCodeBinding> CompileExternalKeyValueRule<T>(Rule rule) =>
            Try(() =>
                {
                    var type = typeof(T);
                    var genericType = Parameter(type);

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