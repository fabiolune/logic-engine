using LogicEngine.Interfaces;
using LogicEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using TinyFp;
using TinyFp.Extensions;
using static System.Array;

namespace LogicEngine;

public class RulesManager<T> : IRulesManager<T> where T : new()
{
    private readonly IRulesCatalogCompiler _catalogCompiler;

    public RulesManager(IRulesCatalogCompiler catalogCompiler)
    {
        _catalogCompiler = catalogCompiler;
        _itemSatisfiesRulesWithMessage = ItemSatisfiesRulesWithMessageAlwaysUnit;
        _itemSatisfiesRules = ItemSatisfiesRulesAlwaysTrue;
    }

    public RulesCatalog Catalog
    {
        set => (_itemSatisfiesRulesWithMessage, _itemSatisfiesRules) =
                    value
                        .Map(_catalogCompiler.CompileCatalog<T>)
                        .ToOption(_ => _.Executables,
                                  _ => _.Executables.Length == 0)
                        .Match(_ => (ItemSatisfiesRulesWithMessageUsingCatalog(_), ItemSatisfiesRulesUsingCatalog(_)),
                               () => (ItemSatisfiesRulesWithMessageAlwaysUnit, ItemSatisfiesRulesAlwaysTrue));
    }

    /// <summary>
    ///     the full rules catalog is satisfied if at least one ruleSet is satisfied (OR);
    ///     a single ruleSet is satisfied if ALL its rules are satisfied (AND)
    ///
    /// The result can be Unit if the rule is matched, otherwise the set of codes associated to the failing rules
    /// </summary>
    /// <param name="item"></param>
    /// <returns><![CDATA[Either<string[], Unit>]]></returns>
    public Either<string[], Unit> ItemSatisfiesRulesWithMessage(T item) => _itemSatisfiesRulesWithMessage(item);

    private Func<T, Either<string[], Unit>> _itemSatisfiesRulesWithMessage;

    private static readonly Func<T, Either<string[], Unit>> ItemSatisfiesRulesWithMessageAlwaysUnit = _ => Unit.Default;

    private static Func<T, Either<string[], Unit>> ItemSatisfiesRulesWithMessageUsingCatalog(Func<T, Either<string, Unit>>[][] rulesCatalog) =>
        item => Loop(rulesCatalog.Where(r => r is not null).ToArray(), item, Empty<string>())
                    .Match(Either<string[], Unit>.Left, 
                           () => Unit.Default);

    private static Option<string[]> Loop(Func<T, Either<string, Unit>>[][] rulesCatalog, T item, string[] errorCodes) =>
        rulesCatalog
            .ToOption(_ => (_.First(), _.Skip(1)), _ => _.Length == 0)
            .Match(_ => EvaluateRuleSet(_.Item1, item)
                            .Map(__ => __.Concat(errorCodes).ToArray())
                            .Bind(__ => Loop(_.Item2.ToArray(), item, __)),
                   () => CloseLoop(errorCodes));

    private static Option<string[]> EvaluateRuleSet(IEnumerable<Func<T, Either<string, Unit>>> ruleset, T item) =>
        ruleset
          .Where(r => r is not null)
          .Select(rule => rule.Invoke(item))
          .Where(_ => _.IsLeft)
          .Select(_ => _.UnwrapLeft())
          .ToArray()
          .ToOption(_ => _.Length == 0);

    private static Option<string[]> CloseLoop(string[] errorCodes) =>
        errorCodes
            .ToOption(_ => _.Where(__ => __ != string.Empty).Distinct().ToArray(),
                      _ => _.Length == 0);

    /// <summary>
    ///     the full rules catalog is satisfied if at least one ruleSet is satisfied (OR)
    ///     a single ruleSet is satisfied if ALL its rules are satisfied (AND)
    /// </summary>
    /// <param name="item"></param>
    /// <returns>bool</returns>
    public bool ItemSatisfiesRules(T item) => _itemSatisfiesRules(item);

    private Func<T, bool> _itemSatisfiesRules;

    private static readonly Func<T, bool> ItemSatisfiesRulesAlwaysTrue = _ => true;

    private static Func<T, bool> ItemSatisfiesRulesUsingCatalog(Func<T, Either<string, Unit>>[][] rulesCatalog) =>
        item => rulesCatalog
            .Where(r => r is not null)
            .Any(_ => _.All(__ => __.Invoke(item).IsRight));

    /// <summary>
    ///     It filters the items keeping only those that satisfy the rules
    /// </summary>
    /// <param name="items"></param>
    /// <returns><![CDATA[IEnumerable<T>]]></returns>
    public IEnumerable<T> Filter(IEnumerable<T> items) => items.Where(ItemSatisfiesRules);

    /// <summary>
    ///     Returns the first item that matches the catalog
    /// </summary>
    /// <param name="items"></param>
    /// <returns>T</returns>
    public T FirstOrDefault(IEnumerable<T> items) => items.FirstOrDefault(ItemSatisfiesRules);
}
