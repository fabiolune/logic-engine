using LogicEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using TinyFp;
using TinyFp.Extensions;
using static LogicEngine.Internals.StaticShared;

namespace LogicEngine;

public record CompiledCatalog<T> : IApplyable<T, IEnumerable<string>> where T : new()
{
    private readonly Func<T, bool> _apply = Functions<T>.AlwaysTrueApply;
    private readonly Func<T, Either<IEnumerable<string>, Unit>> _applyDetailed = Functions<T, IEnumerable<string>>.AlwaysUnitDetailedApply;

    public CompiledCatalog(CompiledRulesSet<T>[] ruleSets) =>
        (_apply, _applyDetailed) = ruleSets
            .Select(s => s.ToOption())
            .Where(c => c.IsSome)
            .Select(s => s.Unwrap())
            .ToOption(e => !e.Any())
            .Map(e => e.ToArray())
            .Map(rs => (GetApplyFromRules(rs), GetDetailedApplyFromRules(rs)))
            .OrElse((Functions<T>.AlwaysTrueApply, Functions<T, IEnumerable<string>>.AlwaysUnitDetailedApply));
    public bool Apply(T item) => _apply(item);

    public Either<IEnumerable<string>, Unit> DetailedApply(T item) => _applyDetailed(item);

    private static Func<T, bool> GetApplyFromRules(CompiledRulesSet<T>[] ruleSets) =>
        item => !ruleSets
            .TakeWhile(s => !s.Apply(item))
            .Any();

    private static Func<T, Either<IEnumerable<string>, Unit>> GetDetailedApplyFromRules(CompiledRulesSet<T>[] ruleSets) =>
        item => ruleSets
            .Select(s => s.DetailedApply(item))
            .FilterLeft()
            .Map<IEnumerable<IEnumerable<string>>, Either<IEnumerable<string>, Unit>>(e => e.ToEither(_ => Unit.Default, _ => _.Any(), e.SelectMany(i => i)));

}
