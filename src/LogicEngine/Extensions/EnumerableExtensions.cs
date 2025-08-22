using LogicEngine.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace LogicEngine.Extensions;

public static class EnumerableExtensions
{
    /// <summary>
    /// Filters the elements of the specified enumerable based on a condition defined by the provided <see
    /// cref="IAppliable{T}"/> implementation.
    /// </summary>
    /// <remarks>This method uses the <see cref="IAppliable{T}.Apply"/> method to determine whether each
    /// element in the source enumerable should be included in the result.</remarks>
    /// <typeparam name="T">The type of elements in the enumerable. Must have a parameterless constructor.</typeparam>
    /// <param name="enumerable">The source enumerable to filter. Cannot be <see langword="null"/>.</param>
    /// <param name="app">An implementation of <see cref="IAppliable{T}"/> that defines the condition to apply to each element. Cannot be
    /// <see langword="null"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing elements from the source enumerable that satisfy the condition
    /// defined by <paramref name="app"/>.</returns>
    public static IEnumerable<T> Filter<T>(this IEnumerable<T> enumerable, IAppliable<T> app) where T : new() =>
        enumerable.Where(app.Apply);

    /// <summary>
    /// Returns the first element in the sequence that satisfies the specified condition, or the default value
    /// if no such element is found.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the sequence. Must have a parameterless constructor.</typeparam>
    /// <param name="this">The sequence to search.</param>
    /// <param name="app">An object implementing <see cref="IAppliable{T}"/> that defines the condition to apply to each element.</param>
    /// <returns>The first element in the sequence that satisfies the condition defined by <paramref name="app"/>.  If no such
    /// element is found, a new instance of <typeparamref name="T"/> is returned.</returns>
    public static T FirstOrDefault<T>(this IEnumerable<T> @this, IAppliable<T> app) where T : new() =>
        @this.FirstOrDefault(app.Apply);
}