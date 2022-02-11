namespace RulesEngine.Internals
{
    internal enum OperatorCategory
    {
        None,
        InternalDirect,
        Direct,
        Enumerable,
        KeyValue,
        InternalEnumerable,
        InternalCrossEnumerable,
        InverseEnumerable
    }
}