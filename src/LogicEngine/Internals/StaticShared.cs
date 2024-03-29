﻿using System;
using TinyFp;

namespace LogicEngine.Internals;

internal static class StaticShared
{
    internal static class Functions<T>
    {
        internal static readonly Func<T, T> Identity = t => t;

        internal static readonly Func<T, bool> AlwaysTrue = Functions<T, bool>.Constant(true);

    }

    internal static class Functions<T1, T2>
    {
        internal static Func<T1, T2> Constant(T2 result) => _ => result;

        internal static readonly Func<T1, Either<T2, Unit>> AlwaysRightEitherUnit =
            Functions<T1, Either<T2, Unit>>.Constant(Either<T2, Unit>.Right(Unit.Default));
    }
}
