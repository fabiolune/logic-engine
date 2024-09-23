using LogicEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using TinyFp;
using TinyFp.Extensions;
using static LogicEngine.Internals.StaticShared;

namespace LogicEngine;

/* The code defines a public record called `CompiledRulesSet<T>`. This record implements three
interfaces: `IAppliable<T>`, `IDetailedAppliable<T, IEnumerable<string>>`, and `IAppliedSelector<T,
string>`. The record has a generic type parameter `T` which must have a parameterless constructor. */
public record CompiledRulesSet<T> :
    IAppliable<T>,
    IDetailedAppliable<T, IEnumerable<string>>,
    IAppliedSelector<T, string> where T : new()
{
    private readonly Func<T, bool> _apply;
    private readonly Func<T, Either<IEnumerable<string>, Unit>> _detailedApply;
    private readonly Func<T, Option<string>> _firstMaching;

    public string Name { get; }

    public CompiledRulesSet(CompiledRule<T>[] rules, string name) =>
        (_apply, _detailedApply, _firstMaching, Name) = rules
            .ToOption(e => e.Length == 0)
            .Map(e => e.ToList())
            .Map(e => (GetApplyFromRules(e), GetDetailedApplyFromRules(e), GetFirstMatchingFromRules(e)))
            .OrElse((Functions<T>.AlwaysTrue, Functions<T, IEnumerable<string>>.AlwaysRightEitherUnit, Functions<T, Option<string>>.Constant(Option<string>.None())))
            .Map(t => (t.Item1, t.Item2, t.Item3, name));

    public bool Apply(T item) => _apply(item);

    public Either<IEnumerable<string>, Unit> DetailedApply(T item) => _detailedApply(item);

    public Option<string> FirstMatching(T item) => _firstMaching(item);

    private static Func<T, bool> GetApplyFromRules(List<CompiledRule<T>> rules) =>
        item => rules.TrueForAll(r => r.Apply(item));

    private static Func<T, Either<IEnumerable<string>, Unit>> GetDetailedApplyFromRules(List<CompiledRule<T>> rules) =>
        item => rules
            .Select(r => r.DetailedApply(item))
            .FilterLeft()
            .Map<IEnumerable<string>, Either<IEnumerable<string>, Unit>>(e => e.ToEither(_ => Unit.Default, _ => _.Any(), e));

    private static Func<T, Option<string>> GetFirstMatchingFromRules(List<CompiledRule<T>> rules) =>
        item => rules
            .Find(r => r.Apply(item))
            .ToOption()
            .Map(r => r.Code);
}