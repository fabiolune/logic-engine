using System;
using System.Collections.Generic;
using System.Linq;
using TinyFp.Extensions;

namespace LogicEngine.Models;

public record RulesCatalog
{
    private readonly IEnumerable<RulesSet> _rulesSets;
    public string Name { get; }

    public IEnumerable<RulesSet> RulesSets
    {
        get => _rulesSets ?? Array.Empty<RulesSet>();
        private init => _rulesSets  = value;
    }

    public RulesCatalog(IEnumerable<RulesSet> rulesSets, string name)
    {
        RulesSets = rulesSets;
        Name = name;
    }

    /// <summary>
    ///     This represents the logical OR between two catalogs
    /// </summary>
    /// <param name="catalog1"></param>
    /// <param name="catalog2"></param>
    /// <returns></returns>
    public static RulesCatalog operator +(RulesCatalog catalog1, RulesCatalog catalog2) =>
        new(catalog1.RulesSets.Concat(catalog2.RulesSets), $"({catalog1.Name} OR {catalog2.Name})");

    /// <summary>
    ///     his represents the logical AND between two catalogs
    /// </summary>
    /// <param name="catalog1"></param>
    /// <param name="catalog2"></param>
    /// <returns></returns>
    public static RulesCatalog operator *(RulesCatalog catalog1, RulesCatalog catalog2) =>
        (catalog1, catalog2)
        .Map(_ => (catalog1.RulesSets, catalog2.RulesSets,
            $"({catalog1.Name} AND {catalog2.Name})"))
        .Map(_ => (from r1 in _.Item1
            from r2 in _.Item2
            select r1 * r2, _.Item3))
        .Map(_ => new RulesCatalog(_.Item1, _.Item2));
}