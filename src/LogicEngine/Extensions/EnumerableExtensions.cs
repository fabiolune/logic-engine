using LogicEngine.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace LogicEngine.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<T> Filter<T>(this IEnumerable<T> @this, IAppliable<T> app) where T : new() =>
        @this.Where(app.Apply);

    public static T FirstOrDefault<T>(this IEnumerable<T> @this, IAppliable<T> app) where T : new() =>
        @this.FirstOrDefault(app.Apply);
}