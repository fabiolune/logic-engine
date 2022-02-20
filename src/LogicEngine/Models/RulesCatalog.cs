using System;
using System.Collections.Generic;
using System.Linq;
using TinyFp.Extensions;

namespace LogicEngine.Models;

public record RulesCatalog
{
    public string Name { get; init; }
    public IEnumerable<RulesSet> RulesSets { get; init; }

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
    public static RulesCatalog operator +(RulesCatalog catalog1, RulesCatalog catalog2) 
        => new((catalog1.RulesSets ?? Array.Empty<RulesSet>()).Union(catalog2.RulesSets ?? Array.Empty<RulesSet>()), $"{catalog1.Name} OR {catalog2.Name}");

    /// <summary>
    ///     his represents the logical AND between two catalogs
    /// </summary>
    /// <param name="catalog1"></param>
    /// <param name="catalog2"></param>
    /// <returns></returns>
    public static RulesCatalog operator *(RulesCatalog catalog1, RulesCatalog catalog2) =>
        (catalog1, catalog2)
        .Map(_ => (catalog1.RulesSets ?? Array.Empty<RulesSet>(), catalog2.RulesSets ?? Array.Empty<RulesSet>(),
            $"{catalog1.Name} AND {catalog2.Name}"))
        .Map(_ => (from r1 in _.Item1
            from r2 in _.Item2
            select new RulesSet
            {
                Description = $"{r1.Description} AND {r2.Description}",
                Rules = (r1.Rules ?? Array.Empty<Rule>()).Union(r2.Rules ?? Array.Empty<Rule>())
            }, _.Item3))
        .Map(_ => new RulesCatalog(_.Item1, _.Item2));
    //return new(
    //    from r1 in catalog1.RulesSets ?? Array.Empty<RulesSet>()
    //    from r2 in catalog2.RulesSets ?? Array.Empty<RulesSet>()
    //    select new RulesSet
    //    {
    //        Description = $"{r1.Description} AND {r2.Description}",
    //        Rules = (r1.Rules ?? Array.Empty<Rule>()).Union(r2.Rules ?? Array.Empty<Rule>())
    //    },
    //    $"{catalog1.Name} AND {catalog2.Name}");
}