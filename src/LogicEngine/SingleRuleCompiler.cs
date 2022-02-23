using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LogicEngine.Interfaces;
using LogicEngine.Internals;
using LogicEngine.Models;
using TinyFp;
using TinyFp.Extensions;
using static System.Globalization.CultureInfo;
using static System.Linq.Expressions.Expression;
using static LogicEngine.Internals.OperationMappings;
using static TinyFp.Prelude;
using Convert = System.Convert;

namespace LogicEngine;

public class SingleRuleCompiler : ISingleRuleCompiler
{
    private static readonly ConstantExpression UnitConstant = Constant(Unit.Default);
    private static readonly Type NewResultType = typeof(Either<string, Unit>);
    private static readonly MethodInfo LeftMethod = NewResultType.GetMethod(nameof(Either<string, Unit>.Left));
    private static readonly MethodInfo RightMethod = NewResultType.GetMethod(nameof(Either<string, Unit>.Right));
    private static readonly MethodCallExpression SuccessUnitExpression = Call(RightMethod, UnitConstant);

    public Option<CompiledRule<T>> Compile<T>(Rule rule) =>
        CreateCompiledRule<T>(rule)
            .Match(_ => _, _ => Option<ExpressionTypeCodeBinding>.None())
            .Map(_ => Lambda<Func<T, Either<string, Unit>>>(
                Condition(_.TestExpression, SuccessUnitExpression, Call(LeftMethod, Constant(_.Code ?? string.Empty))),
                _.TypeExpression))
            .Map(_ => _.Compile())
            .Map(_ => new CompiledRule<T>(_));

    private static Try<Option<ExpressionTypeCodeBinding>> CreateCompiledRule<T>(Rule rule) =>
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

    private static Try<Option<ExpressionTypeCodeBinding>> CompileDirectRule<T>(Rule rule) =>
        Try(() => (rule, typeof(T))
            .Map(_ => (Parameter(_.Item2), GetTypeFromPropertyName<T>(_.rule.Property), _.rule.Property,
                _.rule.Value, _.rule.Code, _.rule.Operator))
            .Map(_ => (_.Item1, _.Item2, _.Property, _.Code, _.Operator, Constant(_.Item2.BaseType == typeof(Enum)
                ? Enum.Parse(_.Item2, _.Value)
                : Convert.ChangeType(_.Value, _.Item2))))
            .Map(_ => (MakeBinary(DirectMapping[_.Operator], Property(_.Item1, _.Property), _.Item6), _.Item1,
                _.Code))
            .Map(_ => new ExpressionTypeCodeBinding(_.Item1, _.Item2, _.Code))
            .Map(Some));

    private static Try<Option<ExpressionTypeCodeBinding>> CompileInternalDirectRule<T>(Rule rule) =>
        Try(() => (rule, Parameter(typeof(T)))
            .Map(_ => (_.rule, _.Item2, Property(_.Item2, _.rule.Property), Property(_.Item2, _.rule.Value)))
            .Map(_ => (_.rule, _.Item2, _.Item3, _.Item4, GetTypeFromPropertyName<T>(_.rule.Property), GetTypeFromPropertyName<T>(_.rule.Value)))
            .ToOption(_ => _.Item5.FullName != _.Item6.FullName)
            .Map(_ => (MakeBinary(InternalDirectMapping[_.rule.Operator], _.Item3, _.Item4), _.Item2, _.rule.Code))
            .Map(_ => new ExpressionTypeCodeBinding(_.Item1, _.Item2, _.Code)));

    private static Try<Option<ExpressionTypeCodeBinding>> CompileEnumerableRule<T>(Rule rule) =>
        Try(() => (rule, typeof(T))
            .Map(_ => (_.rule, Parameter(_.Item2)))
            .Map(_ => (_.rule, _.Item2, GetTypeFromPropertyName<T>(_.rule.Property)))
            .Map(_ => (_.rule, _.Item2, _.Item3,
                _.Item3.IsArray ? _.Item3.GetElementType() : _.Item3.GetGenericArguments().First()))
            .Map(_ => (EnumerableMapping[_.rule.Operator](_.rule, Property(_.Item2, _.rule.Property), _.Item4),
                _.Item2, _.rule.Code))
            .Map(_ => new ExpressionTypeCodeBinding(_.Item1, _.Item2, _.Code))
            .Map(Some));

    private static Type GetTypeFromPropertyName<T>(string name) =>
        typeof(T)
            .GetProperty(name)
            .PropertyType;

    private static Try<Option<ExpressionTypeCodeBinding>> CompileInternalEnumerableRule<T>(Rule rule) =>
        Try(() => (rule, Parameter(typeof(T)))
            .Map(_ => (_.rule, _.Item2, Property(_.Item2, _.rule.Property), Property(_.Item2, _.rule.Value)))
            .Map(_ => (_.rule, _.Item2, _.Item3, _.Item4, GetTypeFromPropertyName<T>(_.rule.Property),
                GetTypeFromPropertyName<T>(_.rule.Value)))
            .Map(_ => (_.rule, _.Item2, _.Item3, _.Item4, _.Item5, _.Item6,
                _.Item5.IsArray ? _.Item5.GetElementType() : _.Item5.GetGenericArguments().First()))
            .ToOption(_ => _.Item6.FullName != _.Item7.FullName)
            .Map(_ => (InternalEnumerableMapping[_.rule.Operator](_.rule, _.Item3, _.Item5, _.Item4, _.Item5, _.Item6), _.Item2, _.rule.Code))
            .Map(_ => new ExpressionTypeCodeBinding(_.Item1, _.Item2, _.Code)));

    private static Try<Option<ExpressionTypeCodeBinding>> CompileInternalCrossEnumerableRule<T>(Rule rule) =>
        Try(() => (rule, Parameter(typeof(T)))
            .Map(_ => (_.rule, _.Item2, Property(_.Item2, _.rule.Property), Property(_.Item2, _.rule.Value)))
            .Map(_ => (_.rule, _.Item2, _.Item3, _.Item4, GetTypeFromPropertyName<T>(_.rule.Property),
                GetTypeFromPropertyName<T>(_.rule.Value)))
            .ToOption(_ => _.Item5.FullName != _.Item6.FullName)
            .Map(_ => (_.rule, _.Item2, _.Item3, _.Item4, _.Item5, _.Item6,
                _.Item5.IsArray ? _.Item5.GetElementType() : _.Item5.GetGenericArguments().First()))
            .Map(_ => (
                InternalCrossEnumerableMapping
                    [_.rule.Operator](_.rule, _.Item3, _.Item5, _.Item4, _.Item6, _.Item7), _.Item2, _.rule.Code))
            .Map(_ => new ExpressionTypeCodeBinding(_.Item1, _.Item2, _.Code)));

    private static Try<Option<ExpressionTypeCodeBinding>> CompileInverseEnumerableRule<T>(Rule rule) =>
        Try(() => rule
            .Map(_ => (_.Property, _.Code, _.Operator, GetTypeFromPropertyName<T>(_.Property), Parameter(typeof(T)),
                _.Value))
            .Map(_ => (ExternalEnumerableMapping[_.Operator](Property(_.Item5, _.Property), _.Item4, NewArrayInit(
                _.Item4, _.Value.Split(',')
                    .Select(v => Convert.ChangeType(v, _.Item4, InvariantCulture))
                    .Select(Constant))), _.Item5, _.Code))
            .Map(_ => new ExpressionTypeCodeBinding(_.Item1, _.Item2, _.Code))
            .Map(Some));

    private static Try<Option<ExpressionTypeCodeBinding>> CompileExternalKeyValueRule<T>(Rule rule) =>
        Try(() =>
            (rule, typeof(T))
            .Map(_ => (_.rule, _.Item2, Parameter(_.Item2)))
            .Map(_ => (ExternalKeyValueMapping[_.rule.Operator](_.Item3, _.rule, _.Item2),
                _.Item3,
                _.rule.Code))
            .Map(_ => new ExpressionTypeCodeBinding(_.Item1, _.Item2, _.Code))
            .Map(Some)
        );

}