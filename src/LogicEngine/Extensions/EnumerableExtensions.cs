using LogicEngine.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace LogicEngine.Extensions;

public static class EnumerableExtensions
{
    /// <summary>
    /// The function is an extension method that filters an IEnumerable based on a given
    /// applicator.
    /// </summary>
    /// <param name="@this">The parameter `@this` is an extension method parameter that represents the
    /// current instance of the `IEnumerable<T>` that the extension method is being called on. It allows
    /// you to use the extension method syntax on any object that implements `IEnumerable<T>`.</param>
    /// <param name="app">The parameter "app" is of type "IAppliable<T>". It is an instance of a class
    /// that implements the "IAppliable<T>" interface. This interface defines a method called "Apply"
    /// which takes an object of type T and returns a boolean value. The "Filter"</param>
    public static IEnumerable<T> Filter<T>(this IEnumerable<T> @this, IAppliable<T> app) where T : new() =>
        @this.Where(app.Apply);

    /// <summary>
    /// The function returns the first element in an enumerable collection that satisfies a given
    /// condition, or a default value if no such element is found.
    /// </summary>
    /// <param name="@this">The parameter `@this` is an extension method that operates on an
    /// `IEnumerable<T>`. It represents the collection of elements on which the method is
    /// called.</param>
    /// <param name="app">The parameter "app" is of type IAppliable<T>. It is an interface that defines
    /// a method called "Apply" which takes a single parameter of type T and returns a boolean
    /// value.</param>
    public static T FirstOrDefault<T>(this IEnumerable<T> @this, IAppliable<T> app) where T : new() =>
        @this.FirstOrDefault(app.Apply);
}