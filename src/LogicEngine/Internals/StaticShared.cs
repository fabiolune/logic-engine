using System;
using TinyFp;

namespace LogicEngine.Internals;

internal static class StaticShared
{
    internal class Functions<T>
    {
        internal static readonly Func<T, T> Identity = t => t;

        internal static readonly Func<T, bool> AlwaysTrueApply = _ => true;
    }

    internal class Functions<T1, T2>
    {
        internal static readonly Func<T1, Either<T2, Unit>> AlwaysUnitDetailedApply =
            _ => Either<T2, Unit>.Right(Unit.Default);
    }
}
