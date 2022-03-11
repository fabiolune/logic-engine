using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LogicEngine.Models;
using TinyFp.Extensions;
using static System.Linq.Expressions.Expression;
using static System.Convert;
using static LogicEngine.Internals.Constants;

namespace LogicEngine.Internals;

internal static class OperationMappings
{
    private static readonly MethodInfo DictionaryGetItem = typeof(Dictionary<string, string>).GetMethod("get_Item");
    private static readonly Type EnumerableType = typeof(Enumerable);
    private static readonly char[] KeysDelimiter = "[".ToCharArray();
    private const char EndBracket = ']';

    internal static readonly ReadOnlyDictionary<OperatorType, ExpressionType> DirectMapping = new(new Dictionary<OperatorType, ExpressionType>
    {
        {OperatorType.Equal, ExpressionType.Equal},
        {OperatorType.GreaterThan, ExpressionType.GreaterThan},
        {OperatorType.GreaterThanOrEqual, ExpressionType.GreaterThanOrEqual},
        {OperatorType.LessThan, ExpressionType.LessThan},
        {OperatorType.LessThanOrEqual, ExpressionType.LessThanOrEqual},
        {OperatorType.NotEqual, ExpressionType.NotEqual}
    });

    internal static readonly ReadOnlyDictionary<OperatorType, ExpressionType> InternalDirectMapping = new(new Dictionary<OperatorType, ExpressionType>
    {
        {OperatorType.InnerEqual, ExpressionType.Equal},
        {OperatorType.InnerGreaterThan, ExpressionType.GreaterThan},
        {OperatorType.InnerGreaterThanOrEqual, ExpressionType.GreaterThanOrEqual},
        {OperatorType.InnerLessThan, ExpressionType.LessThan},
        {OperatorType.InnerLessThanOrEqual, ExpressionType.LessThanOrEqual},
        {OperatorType.InnerNotEqual, ExpressionType.NotEqual}
    });

    internal static readonly ReadOnlyDictionary<OperatorType, Func<Rule, MemberExpression, Type, BinaryExpression>> EnumerableMapping = new(new Dictionary<OperatorType, Func<Rule, MemberExpression, Type, BinaryExpression>>
    {
        {
            OperatorType.Contains, (r, k, s) => MakeBinary(ExpressionType.AndAlso,
                MakeBinary(ExpressionType.NotEqual, k, NullValue),
                Call(EnumerableType, nameof(Enumerable.Contains), new[] {s},
                    k, Constant(ChangeType(r.Value, s))))
        },
        {
            OperatorType.NotContains, (r, k, s) => MakeBinary(ExpressionType.OrElse,
                MakeBinary(ExpressionType.Equal, k, NullValue),
                IsFalse(Call(EnumerableType, nameof(Enumerable.Contains),
                    new[] {s}, k,
                    Constant(ChangeType(r.Value, s)))))
        },
        {
            OperatorType.Overlaps, (r, k, s) => NewArrayInit(s,
                    r.Value
                        .Split(Comma)
                        .Select(v => ChangeType(v, s, CultureInfo.InvariantCulture))
                        .Select(Constant)
                )
                .Map(_ => MakeBinary(ExpressionType.AndAlso,
                    MakeBinary(ExpressionType.AndAlso,
                        MakeBinary(ExpressionType.NotEqual, k, NullValue),
                        MakeBinary(ExpressionType.NotEqual, _, NullValue)),
                    IsTrue(Call(EnumerableType, nameof(Enumerable.Any), new[] { s },
                        Call(EnumerableType, nameof(Enumerable.Intersect), new[] { s }, k, _)))))
        },
        {
            OperatorType.NotOverlaps, (r, k, s) => NewArrayInit(s,
                    r
                        .Value
                        .Split(Comma)
                        .Select(v => ChangeType(v, s, CultureInfo.InvariantCulture))
                        .Select(Constant)
                )
                .Map(_ => MakeBinary(ExpressionType.OrElse,
                    MakeBinary(ExpressionType.OrElse,
                        MakeBinary(ExpressionType.Equal, k, NullValue),
                        MakeBinary(ExpressionType.Equal, _, NullValue)),
                    IsFalse(Call(EnumerableType, nameof(Enumerable.Any),
                        new[] { s },
                        Call(EnumerableType, nameof(Enumerable.Intersect), new[] { s }, k, _)))))
        }
    });

    internal static readonly ReadOnlyDictionary<OperatorType, Func<ParameterExpression, Rule, Type, BinaryExpression>> ExternalKeyValueMapping = new(new Dictionary<OperatorType, Func<ParameterExpression, Rule, Type, BinaryExpression>>
    {
        {
            OperatorType.ContainsKey, (g, r, t) => Property(g, r.Property)
                .Map(_ => MakeBinary(ExpressionType.AndAlso,
                    MakeBinary(ExpressionType.NotEqual, _, NullValue),
                    Call(_, DictionaryContainsKey,
                        Constant(ChangeType(r.Value,
                            t.GetProperty(r.Property).PropertyType.GetGenericArguments()[0])))))
        },
        {
            OperatorType.NotContainsKey, (g, r, t) => Property(g, r.Property)
                .Map(_ => MakeBinary(ExpressionType.OrElse,
                    MakeBinary(ExpressionType.Equal, _, NullValue), IsFalse(
                        Call(_,
                            DictionaryContainsKey,
                            Constant(ChangeType(r.Value,
                                t.GetProperty(r.Property).PropertyType.GetGenericArguments()[0]))))))
        },
        {
            OperatorType.ContainsValue, (g, r, t) => (Property(g, r.Property), r, t)
                .Map(_ => MakeBinary(ExpressionType.AndAlso,
                    MakeBinary(ExpressionType.NotEqual, _.Item1, NullValue), Call(_.Item1,
                        DictionaryContainsValue,
                        Constant(ChangeType(_.r.Value,
                            _.t.GetProperty(_.r.Property).PropertyType.GetGenericArguments()[1])))))
        },
        {
            OperatorType.NotContainsValue, (p, r, t) => (Property(p, r.Property), r, t)
                .Map(_ => MakeBinary(ExpressionType.OrElse,
                    MakeBinary(ExpressionType.Equal, _.Item1, NullValue), IsFalse(
                        Call(_.Item1,
                            DictionaryContainsValue,
                            Constant(ChangeType(_.r.Value,
                                t.GetProperty(_.r.Property).PropertyType.GetGenericArguments()[0]))))))
        },
        {
            OperatorType.KeyContainsValue, (p, r, t) =>
                (p, r, t)
                    .Map(_ => (_.p, _.r, _.t, Parts: _.r.Property
                    .Split(KeysDelimiter, StringSplitOptions.RemoveEmptyEntries)
                    .Select(_ => _.TrimEnd(EndBracket)).ToArray()))
                    .Map(_ => (_.p, _.r, _.t, _.Parts, Parameters: _.t.GetProperty(_.Parts[0]).PropertyType.GetGenericArguments(), PP: Property(_.p, _.Parts[0])))
                    .Map(_ => (MakeBinary(
                        ExpressionType.AndAlso,
                        MakeBinary(ExpressionType.NotEqual, _.PP, NullValue),
                        Call(_.PP, DictionaryContainsKey,
                            Constant(ChangeType(_.Parts[1], _.Parameters[0])))), MakeBinary(ExpressionType.Equal, Call(_.PP, DictionaryGetItem,
                            Constant(ChangeType(_.Parts[1], _.Parameters[0]))),
                        Constant(ChangeType(r.Value, _.Parameters[1])))))
                    .Map(_ => MakeBinary(ExpressionType.AndAlso, _.Item1, _.Item2))
        },
        {
            OperatorType.NotKeyContainsValue, (p, r, t) =>
                (p, r, t)
                    .Map(_ => (_.p, _.r, _.t, Parts: _.r.Property
                    .Split(KeysDelimiter, StringSplitOptions.RemoveEmptyEntries)
                    .Select(_ => _.TrimEnd(EndBracket)).ToArray()))
                    .Map(_ => (_.p, _.r, _.t, _.Parts, Parameters: _.t.GetProperty(_.Parts[0]).PropertyType.GetGenericArguments(), PP: Property(_.p, _.Parts[0])))
                    .Map(_ => (MakeBinary(
                        ExpressionType.OrElse, MakeBinary(ExpressionType.Equal, _.PP, NullValue),
                        IsFalse(Call(_.PP,
                            DictionaryContainsKey,
                            Constant(ChangeType(_.Parts[1], _.Parameters[0]))))), IsFalse(MakeBinary(ExpressionType.Equal, Call(_.PP, DictionaryGetItem, Constant(ChangeType(_.Parts[1], _.Parameters[0]))),
                        Constant(ChangeType(r.Value, _.Parameters[1]))))))
                    .Map(_ => MakeBinary(ExpressionType.OrElse, _.Item1, _.Item2))

        }
    });

    internal static readonly ReadOnlyDictionary<OperatorType, Func<MemberExpression, Type, NewArrayExpression, BinaryExpression>> ExternalEnumerableMapping = new(new Dictionary<OperatorType, Func<MemberExpression, Type, NewArrayExpression, BinaryExpression>>
    {
        {
            OperatorType.IsContained,
            (k, p, ae) => MakeBinary(ExpressionType.AndAlso,
                MakeBinary(ExpressionType.NotEqual, ae, NullValue),
                Call(EnumerableType, nameof(Enumerable.Contains), new[] {p}, ae, k))
        },
        {
            OperatorType.IsNotContained,
            (k, p, ae) => MakeBinary(ExpressionType.OrElse,
                MakeBinary(ExpressionType.Equal, ae, NullValue),
                IsFalse(Call(EnumerableType, nameof(Enumerable.Contains),
                    new[] {p}, ae, k)))
        }
    });

    internal static readonly ReadOnlyDictionary<OperatorType, Func<Rule, MemberExpression, Type, MemberExpression, Type, Type, BinaryExpression>> InternalEnumerableMapping = new(new Dictionary<OperatorType, Func<Rule, MemberExpression, Type, MemberExpression, Type, Type, BinaryExpression>>
    {
        { OperatorType.InnerContains,  (_, k, _, k2, _, svt) => MakeBinary(ExpressionType.AndAlso, MakeBinary(ExpressionType.NotEqual, k, NullValue), Call(EnumerableType, nameof(Enumerable.Contains), new[] { svt }, k, k2))},
        { OperatorType.InnerNotContains, (_, k, _, k2, _, svt) => MakeBinary(ExpressionType.OrElse, MakeBinary(ExpressionType.Equal, k, NullValue), IsFalse(Call(EnumerableType, nameof(Enumerable.Contains), new[] { svt }, k, k2)))}
    });

    internal static readonly ReadOnlyDictionary<OperatorType, Func<Rule, MemberExpression, Type, MemberExpression, Type, Type, BinaryExpression>> InternalCrossEnumerableMapping = new(new Dictionary<OperatorType, Func<Rule, MemberExpression, Type, MemberExpression, Type, Type, BinaryExpression>>
    {
        { OperatorType.InnerOverlaps, (_, k, _, k2, _, svt) => MakeBinary(ExpressionType.AndAlso, MakeBinary(
                ExpressionType.AndAlso,
                MakeBinary(ExpressionType.NotEqual, k, NullValue),
                MakeBinary(ExpressionType.NotEqual, k2, NullValue)
            ), IsTrue(Call(
                EnumerableType,
                nameof(Enumerable.Any),
                new[] { svt },
                Call(EnumerableType, nameof(Enumerable.Intersect), new[] { svt },
                    k, k2)
            )))
        },
        { OperatorType.InnerNotOverlaps, (_, k, _, k2, _, svt) => MakeBinary(ExpressionType.OrElse, MakeBinary(
                ExpressionType.OrElse,
                MakeBinary(ExpressionType.Equal, k, NullValue),
                MakeBinary(ExpressionType.Equal, k2, NullValue)
            ), IsFalse(Call(
                EnumerableType,
                nameof(Enumerable.Any),
                new[] { svt },
                Call(EnumerableType, nameof(Enumerable.Intersect), new[] { svt }, k, k2)
            )))
        }
    });
}