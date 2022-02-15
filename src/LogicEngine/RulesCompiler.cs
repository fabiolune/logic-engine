using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LogicEngine.Interfaces;
using LogicEngine.Internals;
using LogicEngine.Models;
using Serilog;
using TinyFp;
using TinyFp.Extensions;
using static System.Linq.Expressions.Expression;
using static TinyFp.Prelude;
using Convert = System.Convert;

namespace LogicEngine;

public class RulesCompiler : IRulesCompiler
{
    private const string Component = nameof(RulesCompiler);

    private static readonly ConstantExpression UnitConstant = Constant(Unit.Default);

    private static readonly Option<ExpressionTypeCodeBinding> NoneExpressionTypeCodeBinding =
        Option<ExpressionTypeCodeBinding>.None();

    private static readonly Type NewResultType = typeof(Either<string, Unit>);
    private static readonly MethodInfo LeftMethod = NewResultType.GetMethod(nameof(Either<string, Unit>.Left));
    private static readonly MethodInfo RightMethod = NewResultType.GetMethod(nameof(Either<string, Unit>.Right));
    private static readonly MethodCallExpression SuccessUnitExpression = Call(RightMethod, UnitConstant);
    private readonly ILogger _logger;

    public RulesCompiler(ILogger logger)
    {
        _logger = logger;
    }

    public IEnumerable<Func<T, Either<string, Unit>>> CompileRules<T>(IEnumerable<Rule> rules) =>
        rules
            .Select(r => (r, CreateCompiledRule<T>(r)))
            .Select(t => t.Item2.Match(_ => _, e =>
            {
                _logger.Error(e, "{Component} raised an exception with {Message} when compiling {Rule}", Component,
                    e.Message, t.Item1);
                return NoneExpressionTypeCodeBinding;
            }))
            .Where(r => r.IsSome)
            .Select(_ => _.Match(f => f, () => ExpressionTypeCodeBinding.Empty))
            .Select(_ => Lambda<Func<T, Either<string, Unit>>>(
                Condition(_.TestExpression, SuccessUnitExpression, Call(LeftMethod, Constant(_.Code ?? string.Empty))),
                _.TypeExpression))
            .Select(_ => _.Compile());

    private static Try<Option<ExpressionTypeCodeBinding>> CompileDirectRule<T>(Rule rule) =>
        Try(() =>
        {
            var genericType = Parameter(typeof(T));
            var propertyType = GetTypeFromPropertyName<T>(rule.Property);

            var value = Constant(propertyType.BaseType == typeof(Enum)
                ? Enum.Parse(propertyType, rule.Value)
                : Convert.ChangeType(rule.Value, propertyType));

            return Some(new ExpressionTypeCodeBinding
            (
                MakeBinary(OperationMappings.DirectMapping[rule.Operator], Property(genericType, rule.Property), value),
                genericType,
                rule.Code
            ));
        });

    private Try<Option<ExpressionTypeCodeBinding>> CompileInternalDirectRule<T>(Rule rule) =>
        Try(() =>
        {
            const string method = nameof(CompileInternalDirectRule);
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
            (MakeBinary(OperationMappings.InternalDirectMapping[rule.Operator],
                    key, key2),
                genericType,
                rule.Code));
        });

    private Try<Option<ExpressionTypeCodeBinding>> CompileEnumerableRule<T>(Rule rule) =>
        Try(() =>
        {
            var genericType = Parameter(typeof(T));
            var key = Property(genericType, rule.Property);
            var propertyType = GetTypeFromPropertyName<T>(rule.Property);
            var searchValuesType = propertyType.IsArray
                ? propertyType.GetElementType()
                : propertyType.GetGenericArguments().FirstOrDefault();

            return Some(new ExpressionTypeCodeBinding
            (OperationMappings.EnumerableMapping[rule.Operator](rule, key, searchValuesType),
                genericType,
                rule.Code));
        });

    private static Type GetTypeFromPropertyName<T>(string name) =>
        typeof(T)
            .GetProperty(name)
            .PropertyType;

    private Try<Option<ExpressionTypeCodeBinding>> CompileInternalEnumerableRule<T>(Rule rule) =>
        Try(() =>
        {
            const string method = nameof(CompileInternalEnumerableRule);
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
            (
                OperationMappings.InternalEnumerableMapping[rule.Operator](rule, key,
                    propertyType, key2, propertyType2, searchValueType),
                genericType,
                rule.Code
            ));
        });

    private Try<Option<ExpressionTypeCodeBinding>> CompileInternalCrossEnumerableRule<T>(Rule rule) =>
        Try(() =>
        {
            const string method = nameof(CompileInternalCrossEnumerableRule);
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

            return Some(new ExpressionTypeCodeBinding(OperationMappings.InternalCrossEnumerableMapping[rule.Operator](
                    rule, key,
                    propertyType, key2, propertyType2, searchValueType),
                genericType,
                rule.Code)
            );
        });

    private Try<Option<ExpressionTypeCodeBinding>> CompileInverseEnumerableRule<T>(Rule rule) =>
        Try(() =>
        {
            var genericType = Parameter(typeof(T));
            var propertyType = GetTypeFromPropertyName<T>(rule.Property);

            return Some(new ExpressionTypeCodeBinding
            (
                OperationMappings.ExternalEnumerableMapping[rule.Operator](
                    Property(genericType, rule.Property),
                    propertyType, NewArrayInit(propertyType, rule.Value.Split(',')
                        .Select(v => Convert.ChangeType(v, propertyType, CultureInfo.InvariantCulture))
                        .Select(Constant))),
                genericType,
                rule.Code
            ));
        });

    private Try<Option<ExpressionTypeCodeBinding>> CompileExternalKeyValueRule<T>(Rule rule) =>
        Try(() =>
            (rule, typeof(T))
            .Map(_ => (_.Item1, _.Item2, Parameter(_.Item2)))
            .Map(_ => new ExpressionTypeCodeBinding
            (
                OperationMappings.ExternalKeyValueMapping[_.Item1.Operator](_.Item3, _.Item1, _.Item2),
                _.Item3,
                _.Item1.Code
            ))
            .Map(Some)
        );

    private Try<Option<ExpressionTypeCodeBinding>> CreateCompiledRule<T>(Rule rule) =>
        OperatorClassification.GetOperatorType(rule.Operator) switch
        {
            OperatorCategory.Direct => CompileDirectRule<T>(rule),
            OperatorCategory.Enumerable => CompileEnumerableRule<T>(rule),
            OperatorCategory.InternalDirect => CompileInternalDirectRule<T>(rule),
            OperatorCategory.InternalEnumerable => CompileInternalEnumerableRule<T>(rule),
            OperatorCategory.InternalCrossEnumerable => CompileInternalCrossEnumerableRule<T>(rule),
            OperatorCategory.InverseEnumerable => CompileInverseEnumerableRule<T>(rule),
            OperatorCategory.KeyValue => CompileExternalKeyValueRule<T>(rule),
            _ => Try(Option<ExpressionTypeCodeBinding>.None)
        };
}