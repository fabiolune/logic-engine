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

namespace LogicEngine.Internals
{
    internal static class OperationMappings
    {
        private static readonly MethodInfo DictionaryGetItem = typeof(Dictionary<string, string>).GetMethod("get_Item");

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
                    Call(typeof(Enumerable), nameof(Enumerable.Contains), new[] {s},
                        k, Constant(ChangeType(r.Value, s))))
            },
            {
                OperatorType.NotContains, (r, k, s) => MakeBinary(ExpressionType.OrElse,
                    MakeBinary(ExpressionType.Equal, k, NullValue),
                    IsFalse(Call(typeof(Enumerable), nameof(Enumerable.Contains),
                        new[] {s}, k,
                        Constant(ChangeType(r.Value, s)))))
            },
            {
                OperatorType.Overlaps, (r, k, s) =>
                {
                    var ae = NewArrayInit(s,
                        r.Value.Split(',').Select(v => ChangeType(v, s, CultureInfo.InvariantCulture))
                            .Select(Constant));
                    return MakeBinary(ExpressionType.AndAlso,
                        MakeBinary(ExpressionType.AndAlso,
                            MakeBinary(ExpressionType.NotEqual, k, NullValue),
                            MakeBinary(ExpressionType.NotEqual, ae, NullValue)),
                        IsTrue(Call(typeof(Enumerable), nameof(Enumerable.Any), new[] {s},
                            Call(typeof(Enumerable), nameof(Enumerable.Intersect), new[] {s}, k, ae))));
                }
            },
            {
                OperatorType.NotOverlaps, (r, k, s) =>
                {
                    var ae = NewArrayInit(s,
                        r.Value.Split(',').Select(v => ChangeType(v, s, CultureInfo.InvariantCulture))
                            .Select(Constant));
                    return MakeBinary(ExpressionType.OrElse,
                        MakeBinary(ExpressionType.OrElse,
                            MakeBinary(ExpressionType.Equal, k, NullValue),
                            MakeBinary(ExpressionType.Equal, ae, NullValue)),
                        IsFalse(Call(typeof(Enumerable), nameof(Enumerable.Any),
                            new[] {s},
                            Call(typeof(Enumerable), nameof(Enumerable.Intersect), new[] {s}, k,
                                ae))));
                }
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
                OperatorType.ContainsValue, (g, r, t) =>
                {
                    var p = Property(g, r.Property);
                    return MakeBinary(ExpressionType.AndAlso,
                        MakeBinary(ExpressionType.NotEqual, p, NullValue), Call(p,
                            DictionaryContainsValue,
                            Constant(ChangeType(r.Value,
                                t.GetProperty(r.Property).PropertyType.GetGenericArguments()[1]))));
                }
            },
            {
                OperatorType.NotContainsValue, (p, r, t) =>
                {
                    var property = Property(p, r.Property);
                    return MakeBinary(ExpressionType.OrElse,
                        MakeBinary(ExpressionType.Equal, property, NullValue), IsFalse(
                            Call(property,
                                DictionaryContainsValue,
                                Constant(ChangeType(r.Value,
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
                    var parameters = t.GetProperty(parts[0]).PropertyType.GetGenericArguments();
                    var pp = Property(p, parts[0]);
                    return MakeBinary(ExpressionType.AndAlso, MakeBinary(
                            ExpressionType.AndAlso,
                            MakeBinary(ExpressionType.NotEqual, pp, NullValue),
                            Call(pp, DictionaryContainsKey,
                                Constant(ChangeType(parts[1], parameters[0])))),
                        MakeBinary(ExpressionType.Equal, Call(pp, DictionaryGetItem,
                                Constant(ChangeType(parts[1], parameters[0]))),
                            Constant(ChangeType(r.Value, parameters[1]))));
                }
            },
            {
                OperatorType.NotKeyContainsValue, (p, r, t) =>
                {
                    var parts = r
                        .Property
                        .Split("[".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .Select(_ => _.TrimEnd(']')).ToArray();
                    var parameters = t.GetProperty(parts[0]).PropertyType.GetGenericArguments();
                    var pp = Property(p, parts[0]);
                    return MakeBinary(ExpressionType.OrElse, MakeBinary(
                            ExpressionType.OrElse, MakeBinary(ExpressionType.Equal, pp, NullValue),
                            IsFalse(Call(pp,
                                DictionaryContainsKey,
                                Constant(ChangeType(parts[1], parameters[0]))))),
                        IsFalse(MakeBinary(ExpressionType.Equal, Call(pp, DictionaryGetItem, Constant(ChangeType(parts[1], parameters[0]))),
                            Constant(ChangeType(r.Value, parameters[1])))));
                }
            }
        });

        internal static readonly ReadOnlyDictionary<OperatorType, Func<MemberExpression, Type, NewArrayExpression, BinaryExpression>> ExternalEnumerableMapping = new(new Dictionary<OperatorType, Func<MemberExpression, Type, NewArrayExpression, BinaryExpression>>
        {
            {
                OperatorType.IsContained,
                (k, p, ae) => MakeBinary(ExpressionType.AndAlso,
                    MakeBinary(ExpressionType.NotEqual, ae, NullValue),
                    Call(typeof(Enumerable), nameof(Enumerable.Contains), new[] {p}, ae, k))
            },
            {
                OperatorType.IsNotContained,
                (k, p, ae) => MakeBinary(ExpressionType.OrElse,
                    MakeBinary(ExpressionType.Equal, ae, NullValue),
                    IsFalse(Call(typeof(Enumerable), nameof(Enumerable.Contains),
                        new[] {p}, ae, k)))
            }
        });

        internal static readonly ReadOnlyDictionary<OperatorType, Func<Rule, MemberExpression, Type, MemberExpression, Type, Type, BinaryExpression>> InternalEnumerableMapping = new(new Dictionary<OperatorType, Func<Rule, MemberExpression, Type, MemberExpression, Type, Type, BinaryExpression>>
        {
            { OperatorType.InnerContains,  (_, k, _, k2, _, svt) => MakeBinary(ExpressionType.AndAlso, MakeBinary(ExpressionType.NotEqual, k, NullValue), Call(typeof(Enumerable), nameof(Enumerable.Contains), new[] { svt }, k, k2))},
            { OperatorType.InnerNotContains, (_, k, _, k2, _, svt) => MakeBinary(ExpressionType.OrElse, MakeBinary(ExpressionType.Equal, k, NullValue), IsFalse(Call(typeof(Enumerable), nameof(Enumerable.Contains), new[] { svt }, k, k2)))}
        });

        internal static readonly ReadOnlyDictionary<OperatorType, Func<Rule, MemberExpression, Type, MemberExpression, Type, Type, BinaryExpression>> InternalCrossEnumerableMapping = new(new Dictionary<OperatorType, Func<Rule, MemberExpression, Type, MemberExpression, Type, Type, BinaryExpression>>
        {
            { OperatorType.InnerOverlaps, (_, k, _, k2, _, svt) => MakeBinary(ExpressionType.AndAlso, MakeBinary(
                    ExpressionType.AndAlso,
                    MakeBinary(ExpressionType.NotEqual, k, NullValue),
                    MakeBinary(ExpressionType.NotEqual, k2, NullValue)
                ), IsTrue(Call(
                    typeof(Enumerable),
                    nameof(Enumerable.Any),
                    new[] { svt },
                    Call(typeof(Enumerable), nameof(Enumerable.Intersect), new[] { svt },
                        k, k2)
                )))
            },
            { OperatorType.InnerNotOverlaps, (_, k, _, k2, _, svt) => MakeBinary(ExpressionType.OrElse, MakeBinary(
                    ExpressionType.OrElse,
                    MakeBinary(ExpressionType.Equal, k, NullValue),
                    MakeBinary(ExpressionType.Equal, k2, NullValue)
                ), IsFalse(Call(
                    typeof(Enumerable),
                    nameof(Enumerable.Any),
                    new[] { svt },
                    Call(typeof(Enumerable), nameof(Enumerable.Intersect), new[] { svt }, k, k2)
                )))
            }
        });
    }
}