using LogicEngine.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace LogicEngine.Extensions;

public static class EnumerableExtensions
{
    /// <summary>
    /// Filters the collection based on a given <see cref="IAppliable{T}"/> implementation.
    /// It returns an IEnumerable<T> containing only the elements for which the <see cref="IAppliable{T}.Apply(T)"/> method of the IAppliable<T> instance returns true.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="this"></param>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IEnumerable<T> Filter<T>(this IEnumerable<T> @this, IAppliable<T> app) where T : new() =>
        @this.Where(app.Apply);

    /// <summary>
    /// Takes an <see cref="IAppliable{T}"/> instance as a parameter and returns the first element in the collection that satisfies the condition defined by the <see cref="IAppliable{T}.Apply(T)"/> method of the IAppliable<T> instance.
    /// If no such element is found, it returns the default value for the type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="this"></param>
    /// <param name="app"></param>
    /// <returns></returns>
    public static T FirstOrDefault<T>(this IEnumerable<T> @this, IAppliable<T> app) where T : new() =>
        @this.FirstOrDefault(app.Apply);
}