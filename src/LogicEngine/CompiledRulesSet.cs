using LogicEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using TinyFp;
using TinyFp.Extensions;
using static LogicEngine.Internals.StaticShared;

namespace LogicEngine;

public record CompiledRulesSet<T> : IApplyable<T, IEnumerable<string>> where T : new()
{
    private readonly Func<T, bool> _apply = Functions<T>.AlwaysTrueApply;
    private readonly Func<T, Either<IEnumerable<string>, Unit>> _detailedApply = Functions<T, IEnumerable<string>>.AlwaysUnitDetailedApply;

    public CompiledRulesSet(CompiledRule<T>[] rules) =>
        (_apply, _detailedApply) = rules
            .ToOption(e => !e.Any())
            .Map(e => (GetApplyFromRules(e), GetDetailedApplyFromRules(e)))
            .OrElse((Functions<T>.AlwaysTrueApply, Functions<T, IEnumerable<string>>.AlwaysUnitDetailedApply));

    public bool Apply(T item) => _apply(item);

    public Either<IEnumerable<string>, Unit> DetailedApply(T item) => _detailedApply(item);

    // TODO: need to check the fact that rules are really executed only once when _apply is executed
    private static Func<T, bool> GetApplyFromRules(CompiledRule<T>[] rules) =>
        item => !rules
            .ToArray()
            .TakeWhile(r => !r.Apply(item))
            .Any();

    private static Func<T, Either<IEnumerable<string>, Unit>> GetDetailedApplyFromRules(CompiledRule<T>[] rules) =>
        item => rules
            .Select(r => r.DetailedApply(item))
            .FilterLeft()
            .ToArray()
            .Map<IEnumerable<string>, Either<IEnumerable<string>, Unit>>(e => e.ToEither(_ => Unit.Default, _ => _.Any(), e));

    
}