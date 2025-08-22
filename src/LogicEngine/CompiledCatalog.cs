using LogicEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using TinyFp;
using TinyFp.Extensions;
using static LogicEngine.Internals.StaticShared;

namespace LogicEngine;

/// <summary>
/// Represents a compiled catalog of rules that can be applied to items of type <typeparamref name="T"/>.
/// </summary>
/// <remarks>This catalog provides functionality to apply rules to an item, retrieve detailed results of rule
/// application, and identify the first matching rule for a given item. It is constructed from a set of compiled rule
/// sets.</remarks>
/// <typeparam name="T">The type of items to which the catalog's rules are applied. Must have a parameterless constructor.</typeparam>
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
    /// Represents a compiled catalog of rules, providing functionality to apply rules,  retrieve detailed results, and
    /// find the first matching rule.
    /// </summary>
    /// <remarks>The catalog is initialized with the provided rule sets, filtering out any invalid or empty
    /// rule sets. If no valid rule sets are provided, default functions are used for applying rules, retrieving
    /// detailed results, and finding the first matching rule.</remarks>
    /// <param name="ruleSets">An array of compiled rule sets. Each rule set is evaluated and included in the catalog  if it contains valid
    /// rules.</param>
    /// <param name="name">The name of the catalog.</param>
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
    /// Applies the specified operation to the given item and returns the result.
    /// </summary>
    /// <param name="item">The item to which the operation is applied.</param>
    /// <returns><see langword="true"/> if the operation succeeds; otherwise, <see langword="false"/>. </returns>
    public bool Apply(T item) => _apply(item);
    
    /// <summary>
    /// Applies the specified item and returns either a collection of error messages or a success indicator.
    /// </summary>
    /// <remarks>Use this method to apply an item and handle potential errors in a detailed manner. The left
    /// side of the result contains error messages describing the issues encountered, while the right side indicates a
    /// successful operation.</remarks>
    /// <param name="item">The item to be applied. Cannot be null.</param>
    /// <returns>An <see cref="Either{TLeft, TRight}"/> containing a collection of error messages if the operation fails, or a
    /// <see cref="Unit"/> value if the operation succeeds.</returns>
    public Either<IEnumerable<string>, Unit> DetailedApply(T item) => _applyDetailed(item);
    
    /// <summary>
    /// Finds the first matching string for the specified item.
    /// </summary>
    /// <remarks>The method uses the provided item to determine a match and returns the first result.  The
    /// behavior of the matching logic depends on the implementation of the underlying  <c>_firstMatching</c>
    /// function.</remarks>
    /// <param name="item">The item to match against.</param>
    /// <returns>An <see cref="Option{T}"/> containing the first matching string if a match is found;  otherwise, an empty <see
    /// cref="Option{T}"/>.</returns>
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