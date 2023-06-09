using System.Collections.Generic;
using LogicEngine.Interfaces.Managers;

namespace LogicEngine.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<T> Filter<T>(this IEnumerable<T> @this, IRulesCatalogManager<T> manager) where T : new() => manager.Filter(@this);
    public static T FirstOrDefault<T>(this IEnumerable<T> @this, IRulesCatalogManager<T> manager) where T : new() => manager.FirstOrDefault(@this);
}