using LogicEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using TinyFp;
using TinyFp.Extensions;
using static LogicEngine.Internals.StaticShared;

namespace LogicEngine;

public record CompiledCatalog<T> :
    IApplyable<T>,
    IDetailedApplyable<T, IEnumerable<string>>,
    IAppliedSelector<T, string> where T : new()
{
    private readonly Func<T, bool> _apply = Functions<T>.AlwaysTrueApply;
    private readonly Func<T, Either<IEnumerable<string>, Unit>> _applyDetailed = Functions<T, IEnumerable<string>>.AlwaysUnitDetailedApply;
    private readonly Func<T, Option<string>> _firstMaching = Functions<T, Option<string>>.Constant(Option<string>.None());

    public string Name { get; }

    public CompiledCatalog(CompiledRulesSet<T>[] ruleSets, string name) =>
        (_apply, _applyDetailed, _firstMaching, Name) = ruleSets
            .Select(s => s.ToOption())
            .Where(c => c.IsSome)
            .Select(s => s.Unwrap())
            .ToOption(e => !e.Any())
            .Map(e => e.ToArray())
            .Map(rs => (GetApplyFromRules(rs), GetDetailedApplyFromRules(rs), GetFirstMatchingFromRules(rs)))
            .OrElse((Functions<T>.AlwaysTrueApply, Functions<T, IEnumerable<string>>.AlwaysUnitDetailedApply, Functions<T, Option<string>>.Constant(Option<string>.None())))
            .Map(t => (t.Item1, t.Item2, t.Item3, name));

    public bool Apply(T item) => _apply(item);

    public Either<IEnumerable<string>, Unit> DetailedApply(T item) => _applyDetailed(item);

    public Option<string> FirstMatching(T item) => _firstMaching(item);

    private static Func<T, bool> GetApplyFromRules(CompiledRulesSet<T>[] ruleSets) =>
        item => ruleSets.Any(s => s.Apply(item));

    private static Func<T, Either<IEnumerable<string>, Unit>> GetDetailedApplyFromRules(CompiledRulesSet<T>[] ruleSets) =>
        item => ruleSets
            .Select(s => s.DetailedApply(item))
            .Map(r => r.Any(e => e.IsRight)
                ? Either<IEnumerable<string>, Unit>.Right(Unit.Default)
                : r.FilterLeft().SelectMany(_ => _).Map(Either<IEnumerable<string>, Unit>.Left));

    private static Func<T, Option<string>> GetFirstMatchingFromRules(CompiledRulesSet<T>[] ruleSets) =>
        item => ruleSets
            .FirstOrDefault(r => r.Apply(item))
            .ToOption()
            .Map(r => r.Name);
}
