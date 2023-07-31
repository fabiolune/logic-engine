namespace LogicEngine.Internals;

public enum OperatorType
{
    None,
    // ----------------
    /// <summary>
    /// Item parameter is equal to specified constant
    /// </summary>
    Equal,
    // ----------------
    /// <summary>
    /// Item parameter string starts with specified constant
    /// </summary>
    StringStartsWith,
    // ----------------
    /// <summary>
    /// Item parameter string ends with specified constant
    /// </summary>
    StringEndsWith,
    // ----------------
    /// <summary>
    /// Item parameter string contains specified constant
    /// </summary>
    StringContains,
    /// <summary>
    /// Item parameter greater than specified constant
    /// </summary>
    GreaterThan,
    /// <summary>
    /// Item parameter greater or equal than specified constant
    /// </summary>
    GreaterThanOrEqual,
    /// <summary>
    /// Item parameter less than specified constant
    /// </summary>
    LessThan,
    /// <summary>
    /// Item parameter less or equal than specified constant
    /// </summary>
    LessThanOrEqual,
    /// <summary>
    /// Item parameter not equal to specified constant
    /// </summary>
    NotEqual,
    // ----------------
    /// <summary>
    /// Item parameter contains specified constant
    /// </summary>
    Contains,
    /// <summary>
    /// Item parameter not contains specified constant
    /// </summary>
    NotContains,
    /// <summary>
    /// Item parameter has intersection with specified enumerable constant
    /// </summary>
    Overlaps,
    /// <summary>
    /// Item parameter does not have intersections with specified enumerable constant
    /// </summary>
    NotOverlaps,
    // ----------------
    /// <summary>
    /// Item parameter dictionary property contains the given key
    /// </summary>
    ContainsKey,
    /// <summary>
    /// Item parameter dictionary property does not contain the given key
    /// </summary>
    NotContainsKey,
    /// <summary>
    /// Item parameter dictionary property contains the given value
    /// </summary>
    ContainsValue,
    /// <summary>
    /// Item parameter dictionary property does not contain the given value
    /// </summary>
    NotContainsValue,
    /// <summary>
    /// Item parameter dictionary has the given value for the specified key
    /// </summary>
    KeyContainsValue,
    /// <summary>
    /// Item parameter dictionary has not the given value for the specified key
    /// </summary>
    NotKeyContainsValue,
    // ----------------
    /// <summary>
    /// Item parameter is contained into specified constant array
    /// </summary>
    IsContained,
    /// <summary>
    /// Item parameter is not contained into specified constant array
    /// </summary>
    IsNotContained,
    // ----------------
    /// <summary>
    /// Item parameter equal to another item parameter
    /// </summary>
    InnerEqual,
    /// <summary>
    /// Item parameter greater than another item parameter
    /// </summary>
    InnerGreaterThan,
    /// <summary>
    /// Item parameter greater or equal than another item parameter
    /// </summary>
    InnerGreaterThanOrEqual,
    /// <summary>
    /// Item parameter less than another item parameter
    /// </summary>
    InnerLessThan,
    /// <summary>
    /// Item parameter less or equal than another item parameter
    /// </summary>
    InnerLessThanOrEqual,
    /// <summary>
    /// Item parameter not equal to another item parameter
    /// </summary>
    InnerNotEqual,
    // ----------------
    /// <summary>
    /// Item parameter array contains another item parameter
    /// </summary>
    InnerContains,
    /// <summary>
    /// Item parameter array does not contain another item parameter
    /// </summary>
    InnerNotContains,
    /// <summary>
    /// Item parameter array overlaps another array parameter
    /// </summary>
    InnerOverlaps,
    /// <summary>
    /// Item parameter array does not overlap another array parameter
    /// </summary>
    InnerNotOverlaps,
}