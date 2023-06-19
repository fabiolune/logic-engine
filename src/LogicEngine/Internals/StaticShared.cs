using System;
using TinyFp;

namespace LogicEngine.Internals;

internal static class StaticShared
{
    internal class Functions<T>
    {
        internal static readonly Func<T, T> Identity = t => t;

        internal static readonly Func<T, bool> AlwaysTrueApply = Functions<T, bool>.Constant(true);
    }

    internal class Functions<T1, T2>
    {
        internal static Func<T1, T2> Constant(T2 result) => _ => result;

        internal static readonly Func<T1, Either<T2, Unit>> AlwaysUnitDetailedApply =
            Functions<T1, Either<T2, Unit>>.Constant(Either<T2, Unit>.Right(Unit.Default));
    }
}
