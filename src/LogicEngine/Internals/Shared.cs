using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace LogicEngine.Internals;

internal static class Shared
{
    internal const char Comma = ',';

    internal static readonly Expression NullValue = Expression.Constant(null);

    internal static readonly MethodInfo DictionaryContainsKey =
        typeof(IDictionary<string, string>).GetMethod(nameof(IDictionary<string, string>.ContainsKey));

    internal static readonly MethodInfo DictionaryContainsValue =
        typeof(Dictionary<string, string>).GetMethod(nameof(Dictionary<string, string>.ContainsValue));
}