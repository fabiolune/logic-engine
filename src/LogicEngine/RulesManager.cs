using System;
using System.Collections.Generic;
using System.Linq;
using LogicEngine.Interfaces;
using LogicEngine.Models;
using TinyFp;
using TinyFp.Extensions;
using static System.Array;
using static TinyFp.Prelude;

namespace LogicEngine;

public class RulesManager<T> : IRulesManager<T> where T : new()
{
    private readonly IRulesCatalogCompiler _catalogCompiler;

    private static readonly CompiledCatalog<T> EmptyCatalog = new(Empty<Func<T, Either<string, Unit>>[]>());
    private CompiledCatalog<T> _rulesCatalog;

    public RulesManager(IRulesCatalogCompiler catalogCompiler)
    {
        _catalogCompiler = catalogCompiler;
        _rulesCatalog = EmptyCatalog;
    }

    public RulesCatalog Catalog
    {
        set => _rulesCatalog = _catalogCompiler.CompileCatalog<T>(value);
    }

    /// <summary>
    ///     the full rules catalog is satisfied if at least one ruleSet is satisfied (OR);
    ///     a single ruleSet is satisfied iff ALL its rules are satisfied (AND)
    /// </summary>
    /// <param name="item"></param>
    /// <returns>RulesCatalogApplicationResult</returns>
    public Either<IEnumerable<string>, Unit> ItemSatisfiesRulesWithMessage(T item) =>
        (_rulesCatalog.Executables, Empty<string>())
        .Map(_ => Loop(_, item))
        .Bind(_ => _.Item2.ToOption(x => !x.Any()))
        .Map(_ => _.Where(x => x != string.Empty).Distinct())
        .Match(Either<IEnumerable<string>, Unit>.Left, () => Unit.Default);

    /// <summary>
    ///     the full rules catalog is satisfied if at least one ruleSet is satisfied (OR)
    ///     a single ruleSet is satisfied iff ALL its rules are satisfied (AND)
    /// </summary>
    /// <param name="item"></param>
    /// <returns>bool</returns>
    public bool ItemSatisfiesRules(T item) =>
        _rulesCatalog
            .ToOption(_ => _.Executables.Length == 0)
            .Match(c => c.Executables.Any(_ => _.All(f => f.Invoke(item).IsRight)), () => true);

    /// <summary>
    ///     It filters the items keeping only those that satisfy the rules
    /// </summary>
    /// <param name="items"></param>
    /// <returns><![CDATA[IEnumerable<T>]]></returns>
    public IEnumerable<T> Filter(IEnumerable<T> items) => items.Where(ItemSatisfiesRules);

    /// <summary>
    ///     Returns the first item that matches the condition
    /// </summary>
    /// <param name="items"></param>
    /// <returns>T</returns>
    public T FirstOrDefault(IEnumerable<T> items) => items.FirstOrDefault(ItemSatisfiesRules);

    private static Option<(Func<T, Either<string, Unit>>[][], IEnumerable<string>)> Loop(
        (Func<T, Either<string, Unit>>[][], IEnumerable<string>) data, T item) =>
        data
            .ToOption(_ => !_.Item1.Any() && !_.Item2.Any())
            .Bind(_ => _.Map(r => !r.Item1.Any()
                ? Some(r)
                : r
                    .Item1
                    .First()
                    .Select(x => x.Invoke(item))
                    .Where(x => !x.IsRight)
                    .ToOption(x => x.All(y => y.IsRight))
                    .Bind(l => Loop((r.Item1.Skip(1).ToArray(),
                        r.Item2.Concat(l.Where(x => x.IsLeft)
                            .Select(x => x.UnwrapLeft()))), item))));
}