using System.Collections.Generic;
using Fabiolune.BusinessRulesEngine.Interfaces;

namespace Fabiolune.BusinessRulesEngine.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Filter<T>(this IEnumerable<T> @this, IBusinessRulesManager<T> manager) where T : new() => manager.Filter(@this);
        public static T FirstOrDefault<T>(this IEnumerable<T> @this, IBusinessRulesManager<T> manager) where T : new() => manager.FirstOrDefault(@this);
    }
}