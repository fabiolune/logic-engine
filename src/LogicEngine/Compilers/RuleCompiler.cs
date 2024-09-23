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
    /// <param name="Rule">The "Rule" parameter is an input rule that needs to be compiled.
    /// It is of type "Rule".</param>
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
        Try(() => (rule, type: typeof(T))
            .Map(t => (
                Parameter(t.type), 
                GetTypeFromPropertyName<T>(t.rule.Property), 
                t.rule.Property,
                t.rule.Value, 
                t.rule.Code, 
                t.rule.Operator
            ))
            .Map(t => (t.Item1, t.Item2, t.Property, t.Code, t.Operator, Constant(t.Item2.BaseType == typeof(Enum)
                ? Enum.Parse(t.Item2, t.Value)
                : Convert.ChangeType(t.Value, t.Item2))))
            .Map(t => (MakeBinary(DirectMapping[t.Operator], Property(t.Item1, t.Property), t.Item6), t.Item1,
                t.Code)))
            .Map(GetOption);

    private static Option<(BinaryExpression, ParameterExpression, string)> CompileInternalDirectRule<T>(Rule rule) =>
        Try(() => (rule, Parameter(typeof(T)))
            .Map(t => (t.rule, t.Item2, Property(t.Item2, t.rule.Property), Property(t.Item2, t.rule.Value)))
            .Map(t => (t.rule, t.Item2, t.Item3, t.Item4, GetTypeFromPropertyName<T>(t.rule.Property), GetTypeFromPropertyName<T>(t.rule.Value)))
            .ToOption(_ => _.Item5.FullName != _.Item6.FullName)
            .Map(t => (MakeBinary(InternalDirectMapping[t.rule.Operator], t.Item3, t.Item4), t.Item2, t.rule.Code)))
            .Map(GetOption);

    private static Option<(BinaryExpression, ParameterExpression, string)> CompileStringMethodRule<T>(Rule rule) => 
        Try(() => (rule, type: typeof(T))
            .Map(t => (parameter: Parameter(t.type), t.rule))
            .Map(_ => (StringMethodMapping[_.rule.Operator](_.rule, Property(_.parameter, _.rule.Property)), _.parameter, _.rule.Code)))
            .Map(GetOption);

    private static Option<(BinaryExpression, ParameterExpression, string)> CompileEnumerableRule<T>(Rule rule) =>
        Try(() => (rule, type: typeof(T))
                .Map(t => (t.rule, Parameter(t.type)))
                .Map(t => (t.rule, t.Item2, GetTypeFromPropertyName<T>(t.rule.Property)))
                .Map(t => (
                    t.rule, 
                    t.Item2, 
                    t.Item3,
                    t.Item3.IsArray ? t.Item3.GetElementType() : t.Item3.GetGenericArguments()[0]))
                .Map(t => (
                    EnumerableMapping[t.rule.Operator](t.rule, Property(t.Item2, t.rule.Property), t.Item4),
                    t.Item2, 
                    t.rule.Code)))
            .Map(GetOption);

    private static Type GetTypeFromPropertyName<T>(string name) =>
        typeof(T)
            .GetProperty(name)
            .PropertyType;

    private static Option<(BinaryExpression, ParameterExpression, string)> CompileInternalEnumerableRule<T>(Rule rule) =>
        Try(() => (rule, Parameter(typeof(T)))
                .Map(t => (t.rule, t.Item2, Property(t.Item2, t.rule.Property), Property(t.Item2, t.rule.Value)))
                .Map(t => (t.rule, t.Item2, t.Item3, t.Item4, GetTypeFromPropertyName<T>(t.rule.Property),
                    GetTypeFromPropertyName<T>(t.rule.Value)))
                .Map(t => (t.rule, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6,
                    t.Item5.IsArray ? t.Item5.GetElementType() : t.Item5.GetGenericArguments()[0]))
                .ToOption(_ => _.Item6.FullName != _.Item7.FullName)
                .Map(t => (InternalEnumerableMapping[t.rule.Operator](t.rule, t.Item3, t.Item5, t.Item4, t.Item5, t.Item6), t.Item2, t.rule.Code)))
            .Map(GetOption);

    private static Option<(BinaryExpression, ParameterExpression, string)> CompileInternalCrossEnumerableRule<T>(Rule rule) =>
        Try(() => (rule, Parameter(typeof(T)))
                .Map(t => (t.rule, t.Item2, Property(t.Item2, t.rule.Property), Property(t.Item2, t.rule.Value)))
                .Map(t => (t.rule, t.Item2, t.Item3, t.Item4, GetTypeFromPropertyName<T>(t.rule.Property),
                    GetTypeFromPropertyName<T>(t.rule.Value)))
                .ToOption(_ => _.Item5.FullName != _.Item6.FullName)
                .Map(_ => (_.rule, _.Item2, _.Item3, _.Item4, _.Item5, _.Item6,
                    _.Item5.IsArray ? _.Item5.GetElementType() : _.Item5.GetGenericArguments()[0]))
                .Map(_ => (
                    InternalCrossEnumerableMapping
                        [_.rule.Operator](_.rule, _.Item3, _.Item5, _.Item4, _.Item6, _.Item7), _.Item2, _.rule.Code)))
            .Map(GetOption);

    private static Option<(BinaryExpression, ParameterExpression, string)> CompileInverseEnumerableRule<T>(Rule rule) =>
        Try(() => rule
                .Map(r => (r.Property, r.Code, r.Operator, GetTypeFromPropertyName<T>(r.Property), Parameter(typeof(T)),
                    r.Value))
                .Map(_ => (ExternalEnumerableMapping[_.Operator](Property(_.Item5, _.Property), _.Item4, NewArrayInit(
                    _.Item4, _.Value.Split(Constants.Comma)
                        .Select(v => Convert.ChangeType(v, _.Item4, InvariantCulture))
                        .Select(Constant))), _.Item5, _.Code)))
            .Map(GetOption);

    private static Option<(BinaryExpression, ParameterExpression, string)> CompileExternalKeyValueRule<T>(Rule rule) =>
        Try(() =>
                (rule, typeof(T))
                .Map(t => (t.rule, t.Item2, Parameter(t.Item2)))
                .Map(t => (ExternalKeyValueMapping[t.rule.Operator](t.Item3, t.rule, t.Item2),
                    t.Item3,
                    t.rule.Code))
                )
            .Map(GetOption);

    private static Option<(BinaryExpression, ParameterExpression, string)> GetOption(Try<(BinaryExpression, ParameterExpression, string)> input) =>
        input.Match(e => Some(e), _ => Option<(BinaryExpression, ParameterExpression, string)>.None());

    private static Option<(BinaryExpression, ParameterExpression, string)> GetOption(Try<Option<(BinaryExpression, ParameterExpression, string)>> input) =>
        input.Match(StaticShared.Functions<Option<(BinaryExpression, ParameterExpression, string)>>.Identity, e => Option<(BinaryExpression, ParameterExpression, string)>.None());
}