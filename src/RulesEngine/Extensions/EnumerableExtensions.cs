using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using RulesEngine.Interfaces;

namespace RulesEngine.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Filter<T>(this IEnumerable<T> @this, IRulesManager<T> manager) where T : new() => manager.Filter(@this);
        public static T FirstOrDefault<T>(this IEnumerable<T> @this, IRulesManager<T> manager) where T : new() => manager.FirstOrDefault(@this);

        [Pure]
        public static TS FoldWhile<TS, T>(this IEnumerable<T> list, TS state, Func<TS, T, TS> folder, Func<T, bool> predItem)
        {
            foreach (var item in list)
            {
                if (!predItem(item))
                {
                    return state;
                }
                state = folder(state, item);
            }
            return state;
        }
    }
}