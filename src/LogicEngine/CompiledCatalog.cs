using LogicEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using TinyFp;
using TinyFp.Extensions;
using static LogicEngine.Internals.StaticShared;

namespace LogicEngine;

/* The code defines a public record called `CompiledCatalog<T>`. This record implements three
interfaces: `IAppliable<T>`, `IDetailedAppliable<T, IEnumerable<string>>`, and `IAppliedSelector<T,
string>`. */
public record CompiledCatalog<T> :
    IAppliable<T>,
    IDetailedAppliable<T, IEnumerable<string>>,
    IAppliedSelector<T, string> where T : new()
{
    private readonly Func<T, bool> _apply;
    private readonly Func<T, Either<IEnumerable<string>, Unit>> _applyDetailed;
    private readonly Func<T, Option<string>> _firstMatching;

    public string Name { get; }
    /// <summary>
    /// Creates a new compiled catalog
    /// </summary>
    /// <param name="ruleSets"></param>
    /// <param name="name"></param>
    public CompiledCatalog(CompiledRulesSet<T>[] ruleSets, string name) =>
        (_apply, _applyDetailed, _firstMatching, Name) = ruleSets
            .Select(s => s.ToOption())
            .Where(c => c.IsSome)
            .Select(s => s.Unwrap())
            .ToOption(e => !e.Any())
            .Map(e => e.ToList())
            .Map(rs => (GetApplyFromRules(rs), GetDetailedApplyFromRules(rs), GetFirstMatchingFromRules(rs)))
            .OrElse((Functions<T>.AlwaysTrue, Functions<T, IEnumerable<string>>.AlwaysRightEitherUnit, Functions<T, Option<string>>.Constant(Option<string>.None())))
            .Map(t => (t.Item1, t.Item2, t.Item3, name));
    /// <summary>
    /// Applies the catalog to an item by looping over the rules sets
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Apply(T item) => _apply(item);
    /// <summary>
    /// Applies the catalog to an item and returns either a list of strings (the codes of the rules that are not satisfied) or a unit
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public Either<IEnumerable<string>, Unit> DetailedApply(T item) => _applyDetailed(item);
    /// <summary>
    /// Returns the code of the first rule that is satisfied by the item, None if no rule is satisfied
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public Option<string> FirstMatching(T item) => _firstMatching(item);

    private static Func<T, bool> GetApplyFromRules(List<CompiledRulesSet<T>> ruleSets) =>
       item => ruleSets.Exists(s => s.Apply(item));

    private static Func<T, Either<IEnumerable<string>, Unit>> GetDetailedApplyFromRules(List<CompiledRulesSet<T>> ruleSets) =>
        item => ruleSets
            .Select(s => s.DetailedApply(item))
            .Map(r => r.Any(e => e.IsRight)
                ? Either<IEnumerable<string>, Unit>.Right(Unit.Default)
                : r.FilterLeft().SelectMany(_ => _).Map(Either<IEnumerable<string>, Unit>.Left));

    private static Func<T, Option<string>> GetFirstMatchingFromRules(List<CompiledRulesSet<T>> ruleSets) =>
        item => ruleSets
            .Find(r => r.Apply(item))
            .ToOption()
            .Map(r => r.Name);
}