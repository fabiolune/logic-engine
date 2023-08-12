using LogicEngine.Interfaces.Compilers;
using LogicEngine.Internals;
using LogicEngine.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using TinyFp;
using TinyFp.Extensions;
using static LogicEngine.Internals.OperationMappings;
using static System.Globalization.CultureInfo;
using static System.Linq.Expressions.Expression;
using static TinyFp.Prelude;
using Convert = System.Convert;

namespace LogicEngine.Compilers;

public class RuleCompiler : IRuleCompiler
{
    /// <summary>
    /// The function compiles a rule into a lambda expression and returns an option containing the
    /// compiled rule.
    /// </summary>
    /// <param name="Rule">The "Rule" parameter is an input rule that needs to be compiled. It is of
    /// type "Rule".</param>
    public Option<CompiledRule<T>> Compile<T>(Rule rule) where T : new() =>
        rule
            .Map(CreateCompiledRule<T>)
            .Map(t => (exp: Lambda<Func<T, bool>>(t.Item1, t.Item2), code: t.Item3))
            .Map(t => (func: t.exp.Compile(), t.code))
            .Map(t => new CompiledRule<T>(t.func, t.code));

    private static Option<(BinaryExpression, ParameterExpression, string)> CreateCompiledRule<T>(Rule rule) =>
        OperatorClassification.GetOperatorType(rule.Operator) switch
        {
            OperatorCategory.Direct => CompileDirectRule<T>(rule),
            OperatorCategory.StringDirect => CompileStringMethodRule<T>(rule),
            OperatorCategory.Enumerable => CompileEnumerableRule<T>(rule),
            OperatorCategory.InternalDirect => CompileInternalDirectRule<T>(rule),
            OperatorCategory.InternalEnumerable => CompileInternalEnumerableRule<T>(rule),
            OperatorCategory.InternalCrossEnumerable => CompileInternalCrossEnumerableRule<T>(rule),
            OperatorCategory.InverseEnumerable => CompileInverseEnumerableRule<T>(rule),
            OperatorCategory.KeyValue => CompileExternalKeyValueRule<T>(rule),
            _ => Option<(BinaryExpression, ParameterExpression, string)>.None()
        };

    private static Option<(BinaryExpression, ParameterExpression, string)> CompileDirectRule<T>(Rule rule) =>
        Try(() => (rule, typeof(T))
                .Map(_ => (Parameter(_.Item2), GetTypeFromPropertyName<T>(_.rule.Property), _.rule.Property,
                    _.rule.Value, _.rule.Code, _.rule.Operator))
                .Map(_ => (_.Item1, _.Item2, _.Property, _.Code, _.Operator, Constant(_.Item2.BaseType == typeof(Enum)
                    ? Enum.Parse(_.Item2, _.Value)
                    : Convert.ChangeType(_.Value, _.Item2))))
                .Map(_ => (MakeBinary(DirectMapping[_.Operator], Property(_.Item1, _.Property), _.Item6), _.Item1,
                    _.Code)))
            .Map(GetOption);

    private static Option<(BinaryExpression, ParameterExpression, string)> CompileInternalDirectRule<T>(Rule rule) =>
        Try(() => (rule, Parameter(typeof(T)))
                .Map(_ => (_.rule, _.Item2, Property(_.Item2, _.rule.Property), Property(_.Item2, _.rule.Value)))
                .Map(_ => (_.rule, _.Item2, _.Item3, _.Item4, GetTypeFromPropertyName<T>(_.rule.Property), GetTypeFromPropertyName<T>(_.rule.Value)))
                .ToOption(_ => _.Item5.FullName != _.Item6.FullName)
                .Map(_ => (MakeBinary(InternalDirectMapping[_.rule.Operator], _.Item3, _.Item4), _.Item2, _.rule.Code)))
            .Map(GetOption);

    private static Option<(BinaryExpression, ParameterExpression, string)> CompileStringMethodRule<T>(Rule rule) => 
        Try(() => (rule, typeof(T))
            .Map(_ => (parameter: Parameter(_.Item2), _.rule))
            .Map(_ => (StringMethodMapping[_.rule.Operator](_.rule, Property(_.parameter, _.rule.Property)), _.parameter, _.rule.Code)))
            .Map(GetOption);

    private static Option<(BinaryExpression, ParameterExpression, string)> CompileEnumerableRule<T>(Rule rule) =>
        Try(() => (rule, typeof(T))
                .Map(_ => (_.rule, Parameter(_.Item2)))
                .Map(_ => (_.rule, _.Item2, GetTypeFromPropertyName<T>(_.rule.Property)))
                .Map(_ => (_.rule, _.Item2, _.Item3,
                    _.Item3.IsArray ? _.Item3.GetElementType() : _.Item3.GetGenericArguments()[0]))
                .Map(_ => (EnumerableMapping[_.rule.Operator](_.rule, Property(_.Item2, _.rule.Property), _.Item4),
                    _.Item2, _.rule.Code)))
            .Map(GetOption);

    private static Type GetTypeFromPropertyName<T>(string name) =>
        typeof(T)
            .GetProperty(name)
            .PropertyType;

    private static Option<(BinaryExpression, ParameterExpression, string)> CompileInternalEnumerableRule<T>(Rule rule) =>
        Try(() => (rule, Parameter(typeof(T)))
                .Map(_ => (_.rule, _.Item2, Property(_.Item2, _.rule.Property), Property(_.Item2, _.rule.Value)))
                .Map(_ => (_.rule, _.Item2, _.Item3, _.Item4, GetTypeFromPropertyName<T>(_.rule.Property),
                    GetTypeFromPropertyName<T>(_.rule.Value)))
                .Map(_ => (_.rule, _.Item2, _.Item3, _.Item4, _.Item5, _.Item6,
                    _.Item5.IsArray ? _.Item5.GetElementType() : _.Item5.GetGenericArguments()[0]))
                .ToOption(_ => _.Item6.FullName != _.Item7.FullName)
                .Map(_ => (InternalEnumerableMapping[_.rule.Operator](_.rule, _.Item3, _.Item5, _.Item4, _.Item5, _.Item6), _.Item2, _.rule.Code)))
            .Map(GetOption);

    private static Option<(BinaryExpression, ParameterExpression, string)> CompileInternalCrossEnumerableRule<T>(Rule rule) =>
        Try(() => (rule, Parameter(typeof(T)))
                .Map(_ => (_.rule, _.Item2, Property(_.Item2, _.rule.Property), Property(_.Item2, _.rule.Value)))
                .Map(_ => (_.rule, _.Item2, _.Item3, _.Item4, GetTypeFromPropertyName<T>(_.rule.Property),
                    GetTypeFromPropertyName<T>(_.rule.Value)))
                .ToOption(_ => _.Item5.FullName != _.Item6.FullName)
                .Map(_ => (_.rule, _.Item2, _.Item3, _.Item4, _.Item5, _.Item6,
                    _.Item5.IsArray ? _.Item5.GetElementType() : _.Item5.GetGenericArguments()[0]))
                .Map(_ => (
                    InternalCrossEnumerableMapping
                        [_.rule.Operator](_.rule, _.Item3, _.Item5, _.Item4, _.Item6, _.Item7), _.Item2, _.rule.Code)))
            .Map(GetOption);

    private static Option<(BinaryExpression, ParameterExpression, string)> CompileInverseEnumerableRule<T>(Rule rule) =>
        Try(() => rule
                .Map(_ => (_.Property, _.Code, _.Operator, GetTypeFromPropertyName<T>(_.Property), Parameter(typeof(T)),
                    _.Value))
                .Map(_ => (ExternalEnumerableMapping[_.Operator](Property(_.Item5, _.Property), _.Item4, NewArrayInit(
                    _.Item4, _.Value.Split(Constants.Comma)
                        .Select(v => Convert.ChangeType(v, _.Item4, InvariantCulture))
                        .Select(Constant))), _.Item5, _.Code)))
            .Map(GetOption);

    private static Option<(BinaryExpression, ParameterExpression, string)> CompileExternalKeyValueRule<T>(Rule rule) =>
        Try(() =>
                (rule, typeof(T))
                .Map(_ => (_.rule, _.Item2, Parameter(_.Item2)))
                .Map(_ => (ExternalKeyValueMapping[_.rule.Operator](_.Item3, _.rule, _.Item2),
                    _.Item3,
                    _.rule.Code))
                )
            .Map(t => GetOption(t));

    private static Option<(BinaryExpression, ParameterExpression, string)> GetOption(Try<(BinaryExpression, ParameterExpression, string)> input) =>
        input.Match(e => Some(e), _ => Option<(BinaryExpression, ParameterExpression, string)>.None());

    private static Option<(BinaryExpression, ParameterExpression, string)> GetOption(Try<Option<(BinaryExpression, ParameterExpression, string)>> input) =>
        input.Match(StaticShared.Functions<Option<(BinaryExpression, ParameterExpression, string)>>.Identity, e => Option<(BinaryExpression, ParameterExpression, string)>.None());
}