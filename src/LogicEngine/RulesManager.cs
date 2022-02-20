using System;
using System.Collections.Generic;
using System.Linq;
using LogicEngine.Interfaces;
using LogicEngine.Models;
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
                        .ToOption(_ => _.RulesSets == null || 
                                       !_.RulesSets.Any())
                        .Bind(_ => _catalogCompiler
                                        .CompileCatalog<T>(_)
                                        .ToOption(__ => !__.Executables.Any()))
                        .Match(_ => (ItemSatisfiesRulesWithMessageUsingCatalog(_.Executables), ItemSatisfiesRulesUsingCatalog(_.Executables)),
                               () => (ItemSatisfiesRulesWithMessageAlwaysUnit, ItemSatisfiesRulesAlwaysTrue));
    }

    /// <summary>
    ///     the full rules catalog is satisfied if at least one ruleSet is satisfied (OR);
    ///     a single ruleSet is satisfied iff ALL its rules are satisfied (AND)
    /// </summary>
    /// <param name="item"></param>
    /// <returns>RulesCatalogApplicationResult</returns>
    public Either<IEnumerable<string>, Unit> ItemSatisfiesRulesWithMessage(T item) => _itemSatisfiesRulesWithMessage(item);

    private Func<T, Either<IEnumerable<string>, Unit>> _itemSatisfiesRulesWithMessage;

    private static readonly Func<T, Either<IEnumerable<string>, Unit>> ItemSatisfiesRulesWithMessageAlwaysUnit = _ => Unit.Default;

    private static Func<T, Either<IEnumerable<string>, Unit>> ItemSatisfiesRulesWithMessageUsingCatalog(Func<T, Either<string, Unit>>[][] rulesCatalog) =>
        item => Loop(rulesCatalog, item, Empty<string>())
                    .Match(_ => Either<IEnumerable<string>, Unit>.Left(_.Where(__ => __ != string.Empty).Distinct()),
                           () => Unit.Default);
    private static Option<IEnumerable<string>> Loop(IEnumerable<Func<T, Either<string, Unit>>[]> rulesCatalog, T item, IEnumerable<string> errorCodes) =>
        rulesCatalog
            .ToOption(_ => _.First(),
                      _ => !_.Any())
            .Match(_ => EvaluateRuleSet(_, item)
                            .Bind(__ => Loop(rulesCatalog.Skip(1), item, __.Concat(errorCodes))),
                   () => errorCodes.ToOption(_ => !_.Any()));

    private static Option<IEnumerable<string>> EvaluateRuleSet(Func<T, Either<string, Unit>>[] ruleset, T item) =>
        ruleset
          .Select(rule => rule.Invoke(item))
          .Where(_ => _.IsLeft)
          .Select(_ => _.UnwrapLeft())
          .ToOption(_ => !_.Any());

    /// <summary>
    ///     the full rules catalog is satisfied if at least one ruleSet is satisfied (OR)
    ///     a single ruleSet is satisfied iff ALL its rules are satisfied (AND)
    /// </summary>
    /// <param name="item"></param>
    /// <returns>bool</returns>
    public bool ItemSatisfiesRules(T item) => _itemSatisfiesRules(item);

    private Func<T, bool> _itemSatisfiesRules;
    private static readonly Func<T, bool> ItemSatisfiesRulesAlwaysTrue = _ => true;
    private static Func<T, bool> ItemSatisfiesRulesUsingCatalog(Func<T, Either<string, Unit>>[][] rulesCatalog) =>
        item => rulesCatalog.Any(_ => _.All(__ => __.Invoke(item).IsRight));

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
}
