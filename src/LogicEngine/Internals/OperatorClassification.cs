using System.Collections.Generic;

namespace LogicEngine.Internals;

internal static class OperatorClassification
{
    private static readonly IDictionary<OperatorType, OperatorCategory> TypeCategoryMapping = new Dictionary<OperatorType, OperatorCategory>
    {
        {OperatorType.None,  OperatorCategory.None},
        // ------------------------------------------
        {OperatorType.Equal, OperatorCategory.Direct },
        {OperatorType.GreaterThan, OperatorCategory.Direct },
        {OperatorType.GreaterThanOrEqual, OperatorCategory.Direct },
        {OperatorType.LessThan, OperatorCategory.Direct },
        {OperatorType.LessThanOrEqual, OperatorCategory.Direct },
        {OperatorType.NotEqual, OperatorCategory.Direct },
        // ------------------------------------------
        {OperatorType.StringStartsWith, OperatorCategory.StringMethod },
        {OperatorType.StringEndsWith, OperatorCategory.StringMethod },
        {OperatorType.StringContains, OperatorCategory.StringMethod },
        {OperatorType.StringRegexIsMatch, OperatorCategory.StringMethod },
        // ------------------------------------------
        {OperatorType.Contains, OperatorCategory.Enumerable },
        {OperatorType.NotContains, OperatorCategory.Enumerable },
        {OperatorType.Overlaps, OperatorCategory.Enumerable },
        {OperatorType.NotOverlaps, OperatorCategory.Enumerable },
        // ------------------------------------------
        {OperatorType.ContainsKey, OperatorCategory.KeyValue },
        {OperatorType.NotContainsKey, OperatorCategory.KeyValue },
        {OperatorType.ContainsValue, OperatorCategory.KeyValue },
        {OperatorType.NotContainsValue, OperatorCategory.KeyValue },
        {OperatorType.KeyContainsValue, OperatorCategory.KeyValue },
        {OperatorType.NotKeyContainsValue, OperatorCategory.KeyValue },
        // ------------------------------------------
        {OperatorType.IsContained, OperatorCategory.InverseEnumerable },
        {OperatorType.IsNotContained, OperatorCategory.InverseEnumerable },
        // ------------------------------------------
        {OperatorType.InnerEqual, OperatorCategory.InternalDirect},
        {OperatorType.InnerGreaterThan, OperatorCategory.InternalDirect},
        {OperatorType.InnerGreaterThanOrEqual, OperatorCategory.InternalDirect},
        {OperatorType.InnerLessThan, OperatorCategory.InternalDirect},
        {OperatorType.InnerLessThanOrEqual, OperatorCategory.InternalDirect},
        {OperatorType.InnerNotEqual, OperatorCategory.InternalDirect},
        // ------------------------------------------
        {OperatorType.InnerContains, OperatorCategory.InternalEnumerable},
        {OperatorType.InnerNotContains, OperatorCategory.InternalEnumerable},
        // ------------------------------------------
        {OperatorType.InnerOverlaps, OperatorCategory.InternalCrossEnumerable},
        {OperatorType.InnerNotOverlaps, OperatorCategory.InternalCrossEnumerable }
    };

    internal static OperatorCategory GetOperatorType(OperatorType type)
        => TypeCategoryMapping.TryGetValue(type, out var category) ? category : OperatorCategory.None;
}