using LogicEngine.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace LogicEngine.Extensions;

public static class EnumerableExtensions
{
    /// <summary>
    /// Filters the collection based on a given <see cref="IAppliable{T}"/> implementation. It returns only the elements on which <see cref="IAppliable{T}.Apply"/> returns true
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerable"></param>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IEnumerable<T> Filter<T>(this IEnumerable<T> enumerable, IAppliable<T> app) where T : new() =>
        enumerable.Where(app.Apply);

    /// <summary>
    /// Takes an <see cref="IAppliable{T}"/> instance and returns the first element in the collection that satisfies the condition defined by <see cref="IAppliable{T}.Apply"/>. If no such element is found, it returns the default value for the type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="this"></param>
    /// <param name="app"></param>
    /// <returns></returns>
    public static T FirstOrDefault<T>(this IEnumerable<T> @this, IAppliable<T> app) where T : new() =>
        @this.FirstOrDefault(app.Apply);
}