using LogicEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using TinyFp;
using TinyFp.Extensions;
using static LogicEngine.Internals.StaticShared;

namespace LogicEngine;

/// <summary>
/// Represents a compiled set of rules that can be applied to items of type <typeparamref name="T"/>.
/// </summary>
/// <remarks>This class provides functionality to apply a set of compiled rules to an item, check detailed results
/// of rule application, and retrieve the first matching rule. It implements the <see cref="IAppliable{T}"/>, <see
/// cref="IDetailedAppliable{T, IEnumerable{string}}"/>, and <see cref="IAppliedSelector{T, string}"/>
/// interfaces.</remarks>
/// <typeparam name="T">The type of items to which the rules are applied. Must have a parameterless constructor.</typeparam>
public record CompiledRulesSet<T> :
    IAppliable<T>,
    IDetailedAppliable<T, IEnumerable<string>>,
    IAppliedSelector<T, string> where T : new()
{
    private readonly Func<T, bool> _apply;
    private readonly Func<T, Either<IEnumerable<string>, Unit>> _detailedApply;
    private readonly Func<T, Option<string>> _firstMaching;

    public string Name { get; }
    /// <summary>
    /// Represents a compiled set of rules that can be applied to evaluate conditions and retrieve results.
    /// </summary>
    /// <remarks>The <see cref="CompiledRulesSet{T}"/> class initializes a set of rules that can be applied to
    /// input data. If the provided <paramref name="rules"/> array is empty, default functions are used for rule
    /// evaluation.</remarks>
    /// <param name="rules">An array of compiled rules to include in the set. Must not be empty. If empty, default functions are used.</param>
    /// <param name="name">The name of the compiled rules set. This is used to identify the set.</param>
    public CompiledRulesSet(CompiledRule<T>[] rules, string name) =>
        (_apply, _detailedApply, _firstMaching, Name) = rules
            .ToOption(e => e.Length == 0)
            .Map(e => e.ToList())
            .Map(e => (GetApplyFromRules(e), GetDetailedApplyFromRules(e), GetFirstMatchingFromRules(e)))
            .OrElse((Functions<T>.AlwaysTrue, Functions<T, IEnumerable<string>>.AlwaysRightEitherUnit, Functions<T, Option<string>>.Constant(Option<string>.None())))
            .Map(t => (t.Item1, t.Item2, t.Item3, name));

    /// <summary>
    /// Applies the specified operation to the given item and returns the result.
    /// </summary>
    /// <param name="item">The item to which the operation is applied.</param>
    /// <returns><see langword="true"/> if the operation succeeds; otherwise, <see langword="false"/>. </returns>
    public bool Apply(T item) => _apply(item);
    
    /// <summary>
    /// Applies the specified operation to the given item and returns either a collection of error messages or a success
    /// indicator.
    /// </summary>
    /// <remarks>Use this method to perform an operation on the provided item while capturing detailed error
    /// information in case of failure. The returned <see cref="Either{TLeft, TRight}"/> allows callers to handle
    /// success and failure cases explicitly.</remarks>
    /// <param name="item">The item to which the operation is applied. Cannot be null.</param>
    /// <returns>An <see cref="Either{TLeft, TRight}"/> containing a collection of error messages if the operation fails,  or a
    /// <see cref="Unit"/> value indicating success if the operation completes successfully.</returns>
    public Either<IEnumerable<string>, Unit> DetailedApply(T item) => _detailedApply(item);
    
    /// <summary>
    /// Finds the first matching string for the specified item.
    /// </summary>
    /// <remarks>The method uses the provided item to determine a match and returns the first result. If no
    /// match is found, the returned <see cref="Option{T}"/> will be empty.</remarks>
    /// <param name="item">The item to match against.</param>
    /// <returns>An <see cref="Option{T}"/> containing the first matching string if a match is found;  otherwise, an empty <see
    /// cref="Option{T}"/>.</returns>
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