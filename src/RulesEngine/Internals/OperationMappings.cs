using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using RulesEngine.Models;

namespace RulesEngine.Internals
{
    internal static class OperationMappings
    {
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
                OperatorType.Contains, (r, k, s) => Expression.MakeBinary(ExpressionType.AndAlso,
                    Expression.MakeBinary(ExpressionType.NotEqual, k, Constants.NullValue),
                    Expression.Call(typeof(Enumerable), nameof(Enumerable.Contains), new[] {s},
                        k, Expression.Constant(Convert.ChangeType(r.Value, s))))
            },
            {
                OperatorType.NotContains, (r, k, s) => Expression.MakeBinary(ExpressionType.OrElse,
                    Expression.MakeBinary(ExpressionType.Equal, k, Constants.NullValue),
                    Expression.IsFalse(Expression.Call(typeof(Enumerable), nameof(Enumerable.Contains),
                        new[] {s}, k,
                        Expression.Constant(Convert.ChangeType(r.Value, s)))))
            },
            {
                OperatorType.Overlaps, (r, k, s) =>
                {
                    var ae = Expression.NewArrayInit(s,
                        r.Value.Split(',').Select(v => Convert.ChangeType(v, s, CultureInfo.InvariantCulture))
                            .Select(Expression.Constant));
                    return Expression.MakeBinary(ExpressionType.AndAlso,
                        Expression.MakeBinary(ExpressionType.AndAlso,
                            Expression.MakeBinary(ExpressionType.NotEqual, k, Constants.NullValue),
                            Expression.MakeBinary(ExpressionType.NotEqual, ae, Constants.NullValue)),
                        Expression.IsTrue(Expression.Call(typeof(Enumerable), nameof(Enumerable.Any), new[] {s},
                            Expression.Call(typeof(Enumerable), nameof(Enumerable.Intersect), new[] {s}, k,
                                ae))));
                }
            },
            {
                OperatorType.NotOverlaps, (r, k, s) =>
                {
                    var ae = Expression.NewArrayInit(s,
                        r.Value.Split(',').Select(v => Convert.ChangeType(v, s, CultureInfo.InvariantCulture))
                            .Select(Expression.Constant));
                    return Expression.MakeBinary(ExpressionType.OrElse,
                        Expression.MakeBinary(ExpressionType.OrElse,
                            Expression.MakeBinary(ExpressionType.Equal, k, Constants.NullValue),
                            Expression.MakeBinary(ExpressionType.Equal, ae, Constants.NullValue)),
                        Expression.IsFalse(Expression.Call(typeof(Enumerable), nameof(Enumerable.Any),
                            new[] {s},
                            Expression.Call(typeof(Enumerable), nameof(Enumerable.Intersect), new[] {s}, k,
                                ae))));
                }
            }
        });

        internal static readonly ReadOnlyDictionary<OperatorType, Func<ParameterExpression, Rule, Type, BinaryExpression>> ExternalKeyValueMapping = new(new Dictionary<OperatorType, Func<ParameterExpression, Rule, Type, BinaryExpression>>
        {
            {
                OperatorType.ContainsKey, (g, r, t) =>
                {
                    var p = Expression.Property(g, r.Property);
                    return Expression.MakeBinary(ExpressionType.AndAlso,
                        Expression.MakeBinary(ExpressionType.NotEqual, p, Constants.NullValue),
                        Expression.Call(p, Constants.DictionaryContainsKey,
                            Expression.Constant(Convert.ChangeType(r.Value,
                                t.GetProperty(r.Property).PropertyType.GetGenericArguments()[0]))));
                }
            },
            {
                OperatorType.NotContainsKey, (g, r, t) =>
                {
                    var property = Expression.Property(g, r.Property);
                    return Expression.MakeBinary(ExpressionType.OrElse,
                        Expression.MakeBinary(ExpressionType.Equal, property, Constants.NullValue), Expression.IsFalse(
                            Expression.Call(property,
                                Constants.DictionaryContainsKey,
                                Expression.Constant(Convert.ChangeType(r.Value,
                                    t.GetProperty(r.Property).PropertyType.GetGenericArguments()[0])))));
                }
            },
            {
                OperatorType.ContainsValue, (g, r, t) =>
                {
                    var p = Expression.Property(g, r.Property);
                    return Expression.MakeBinary(ExpressionType.AndAlso,
                        Expression.MakeBinary(ExpressionType.NotEqual, p, Constants.NullValue), Expression.Call(p,
                            Constants.DictionaryContainsValue,
                            Expression.Constant(Convert.ChangeType(r.Value,
                                t.GetProperty(r.Property).PropertyType.GetGenericArguments()[1]))));
                }
            },
            {
                OperatorType.NotContainsValue, (p, r, t) =>
                {
                    var property = Expression.Property(p, r.Property);
                    return Expression.MakeBinary(ExpressionType.OrElse,
                        Expression.MakeBinary(ExpressionType.Equal, property, Constants.NullValue), Expression.IsFalse(
                            Expression.Call(property,
                                Constants.DictionaryContainsValue,
                                Expression.Constant(Convert.ChangeType(r.Value,
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
                    var pp = Expression.Property(p, parts[0]);
                    return Expression.MakeBinary(ExpressionType.AndAlso, Expression.MakeBinary(
                            ExpressionType.AndAlso,
                            Expression.MakeBinary(ExpressionType.NotEqual, pp, Constants.NullValue),
                            Expression.Call(pp, Constants.DictionaryContainsKey,
                                Expression.Constant(Convert.ChangeType(parts[1], parameters[0])))),
                        Expression.MakeBinary(ExpressionType.Equal, Expression.Call(pp, typeof(Dictionary<string, string>).GetMethod("get_Item"),
                                Expression.Constant(Convert.ChangeType(parts[1], parameters[0]))),
                            Expression.Constant(Convert.ChangeType(r.Value, parameters[1]))));
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
                    var pp = Expression.Property(p, parts[0]);
                    return Expression.MakeBinary(ExpressionType.OrElse, Expression.MakeBinary(
                            ExpressionType.OrElse, Expression.MakeBinary(ExpressionType.Equal, pp, Constants.NullValue),
                            Expression.IsFalse(Expression.Call(pp,
                                Constants.DictionaryContainsKey,
                                Expression.Constant(Convert.ChangeType(parts[1], parameters[0]))))),
                        Expression.IsFalse(Expression.MakeBinary(ExpressionType.Equal, Expression.Call(pp,
                                typeof(Dictionary<string, string>).GetMethod("get_Item"),
                                Expression.Constant(Convert.ChangeType(parts[1], parameters[0]))),
                            Expression.Constant(Convert.ChangeType(r.Value, parameters[1])))));
                }
            }
        });

        internal static readonly ReadOnlyDictionary<OperatorType, Func<MemberExpression, Type, NewArrayExpression, BinaryExpression>> ExternalEnumerableMapping = new(new Dictionary<OperatorType, Func<MemberExpression, Type, NewArrayExpression, BinaryExpression>>
        {
            {
                OperatorType.IsContained,
                (k, p, ae) => Expression.MakeBinary(ExpressionType.AndAlso,
                    Expression.MakeBinary(ExpressionType.NotEqual, ae, Constants.NullValue),
                    Expression.Call(typeof(Enumerable), nameof(Enumerable.Contains), new[] {p}, ae, k))
            },
            {
                OperatorType.IsNotContained,
                (k, p, ae) => Expression.MakeBinary(ExpressionType.OrElse,
                    Expression.MakeBinary(ExpressionType.Equal, ae, Constants.NullValue),
                    Expression.IsFalse(Expression.Call(typeof(Enumerable), nameof(Enumerable.Contains),
                        new[] {p}, ae, k)))
            }
        });

        internal static readonly ReadOnlyDictionary<OperatorType, Func<Rule, MemberExpression, Type, MemberExpression, Type, Type, BinaryExpression>> InternalEnumerableMapping = new(new Dictionary<OperatorType, Func<Rule, MemberExpression, Type, MemberExpression, Type, Type, BinaryExpression>>
        {
            { OperatorType.InnerContains,  (r, k, pt, k2, pt2, svt) => Expression.MakeBinary(ExpressionType.AndAlso, Expression.MakeBinary(ExpressionType.NotEqual, k, Constants.NullValue), Expression.Call(typeof(Enumerable), nameof(Enumerable.Contains), new[] { svt }, k, k2))},
            { OperatorType.InnerNotContains, (r, k, pt, k2, pt2, svt) => Expression.MakeBinary(ExpressionType.OrElse, Expression.MakeBinary(ExpressionType.Equal, k, Constants.NullValue), Expression.IsFalse(Expression.Call(typeof(Enumerable), nameof(Enumerable.Contains), new[] { svt }, k, k2)))}
        });

        internal static readonly ReadOnlyDictionary<OperatorType, Func<Rule, MemberExpression, Type, MemberExpression, Type, Type, BinaryExpression>> InternalCrossEnumerableMapping = new(new Dictionary<OperatorType, Func<Rule, MemberExpression, Type, MemberExpression, Type, Type, BinaryExpression>>
        {
            { OperatorType.InnerOverlaps, (r, k, pt, k2, pt2, svt) => Expression.MakeBinary(ExpressionType.AndAlso, Expression.MakeBinary(
                    ExpressionType.AndAlso,
                    Expression.MakeBinary(ExpressionType.NotEqual, k, Constants.NullValue),
                    Expression.MakeBinary(ExpressionType.NotEqual, k2, Constants.NullValue)
                ), Expression.IsTrue(Expression.Call(
                    typeof(Enumerable),
                    nameof(Enumerable.Any),
                    new[] { svt },
                    Expression.Call(typeof(Enumerable), nameof(Enumerable.Intersect), new[] { svt },
                        k, k2)
                )))
            },
            { OperatorType.InnerNotOverlaps, (r, k, pt, k2, pt2, svt) => Expression.MakeBinary(ExpressionType.OrElse, Expression.MakeBinary(
                    ExpressionType.OrElse,
                    Expression.MakeBinary(ExpressionType.Equal, k, Constants.NullValue),
                    Expression.MakeBinary(ExpressionType.Equal, k2, Constants.NullValue)
                ), Expression.IsFalse(Expression.Call(
                    typeof(Enumerable),
                    nameof(Enumerable.Any),
                    new[] { svt },
                    Expression.Call(typeof(Enumerable), nameof(Enumerable.Intersect), new[] { svt }, k, k2)
                )))
            }
        });
    }
}