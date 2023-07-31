namespace LogicEngine.Internals;

internal enum OperatorCategory
{
    None,
    InternalDirect,
    Direct,
    StringMethod,
    Enumerable,
    KeyValue,
    InternalEnumerable,
    InternalCrossEnumerable,
    InverseEnumerable
}