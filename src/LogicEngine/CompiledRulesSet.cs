using LogicEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using TinyFp;
using TinyFp.Extensions;
using static LogicEngine.Internals.StaticShared;

namespace LogicEngine;

public record CompiledRulesSet<T> :
    IApplyable<T>,
    IDetailedApplyable<T, IEnumerable<string>>,
    IAppliedSelector<T, string> where T : new()
{
    private readonly Func<T, bool> _apply = Functions<T>.AlwaysTrueApply;
    private readonly Func<T, Either<IEnumerable<string>, Unit>> _detailedApply = Functions<T, IEnumerable<string>>.AlwaysUnitDetailedApply;
    private readonly Func<T, Option<string>> _firstMaching = Functions<T, Option<string>>.Constant(Option<string>.None());

    public string Name { get; }

    public CompiledRulesSet(CompiledRule<T>[] rules, string name) =>
        (_apply, _detailedApply, _firstMaching, Name) = rules
            .ToOption(e => !e.Any())
            .Map(e => (GetApplyFromRules(e), GetDetailedApplyFromRules(e), GetFirstMatchingFromRules(e)))
            .OrElse((Functions<T>.AlwaysTrueApply, Functions<T, IEnumerable<string>>.AlwaysUnitDetailedApply, Functions<T, Option<string>>.Constant(Option<string>.None())))
            .Map(t => (t.Item1, t.Item2, t.Item3, name));


    public bool Apply(T item) => _apply(item);

    public Either<IEnumerable<string>, Unit> DetailedApply(T item) => _detailedApply(item);

    public Option<string> FirstMatching(T item) => _firstMaching(item);

    // TODO: need to check the performances with and without .ToArray()
    private static Func<T, bool> GetApplyFromRules(CompiledRule<T>[] rules) =>
        item => rules.All(r => r.Apply(item));

    private static Func<T, Either<IEnumerable<string>, Unit>> GetDetailedApplyFromRules(CompiledRule<T>[] rules) =>
        item => rules
            .Select(r => r.DetailedApply(item))
            .FilterLeft()
            .ToArray()
            .Map<IEnumerable<string>, Either<IEnumerable<string>, Unit>>(e => e.ToEither(_ => Unit.Default, _ => _.Any(), e));

    private static Func<T, Option<string>> GetFirstMatchingFromRules(CompiledRule<T>[] rules) =>
        item => rules
            .FirstOrDefault(r => r.Apply(item))
            .ToOption()
            .Map(r => r.Code);
}