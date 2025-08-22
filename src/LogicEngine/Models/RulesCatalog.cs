using System.Collections.Generic;
using System.Linq;
using TinyFp.Extensions;

namespace LogicEngine.Models;

public record RulesCatalog
{
    private readonly IEnumerable<RulesSet> _rulesSets;
    public string Name { get; }
    /// <summary>
    /// Rules sets that make up the catalog
    /// </summary>
    public readonly IEnumerable<RulesSet> RulesSets
    {
        get => _rulesSets ?? [];
        private init => _rulesSets = value;
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="RulesCatalog"/> class with the specified rules sets and name.
    /// </summary>
    /// <param name="rulesSets">A collection of <see cref="RulesSet"/> objects that define the rules contained in the catalog. This parameter
    /// cannot be null.</param>
    /// <param name="name">The name of the rules catalog. This parameter cannot be null or empty.</param>
    public RulesCatalog(IEnumerable<RulesSet> rulesSets, string name)
    {
        RulesSets = rulesSets;
        Name = name;
    }

    /// <summary>
    /// Combines two <see cref="RulesCatalog"/> instances into a single catalog containing the rules from both.
    /// </summary>
    /// <remarks>The resulting catalog will include all rule sets from <paramref name="catalog1"/> and
    /// <paramref name="catalog2"/>. The name of the resulting catalog will be a combination of the names of the input
    /// catalogs, formatted as "(Name1 OR Name2)".</remarks>
    /// <param name="catalog1">The first <see cref="RulesCatalog"/> to combine.</param>
    /// <param name="catalog2">The second <see cref="RulesCatalog"/> to combine.</param>
    /// <returns>A new <see cref="RulesCatalog"/> that contains the combined rule sets from both catalogs.</returns>
    public static RulesCatalog operator +(RulesCatalog catalog1, RulesCatalog catalog2) =>
        new(catalog1.RulesSets.Concat(catalog2.RulesSets), $"({catalog1.Name} OR {catalog2.Name})");

    /// <summary>
    /// Combines two <see cref="RulesCatalog"/> instances by applying a logical AND operation to their respective rule
    /// sets.
    /// </summary>
    /// <remarks>This operator creates a new <see cref="RulesCatalog"/> where each rule in  <paramref
    /// name="catalog1"/> is combined with each rule in <paramref name="catalog2"/>  using their respective
    /// multiplication logic. The resulting catalog's name reflects  the combination of the two input
    /// catalogs.</remarks>
    /// <param name="catalog1">The first <see cref="RulesCatalog"/> to combine.</param>
    /// <param name="catalog2">The second <see cref="RulesCatalog"/> to combine.</param>
    /// <returns>A new <see cref="RulesCatalog"/> instance containing the combined rule sets of  <paramref name="catalog1"/> and
    /// <paramref name="catalog2"/>, with a name indicating  the logical AND operation.</returns>
    public static RulesCatalog operator *(RulesCatalog catalog1, RulesCatalog catalog2) =>
        (catalog1, catalog2)
        .Map(_ => (catalog1.RulesSets, catalog2.RulesSets,
            $"({catalog1.Name} AND {catalog2.Name})"))
        .Map(_ => (from r1 in _.Item1
                   from r2 in _.Item2
                   select r1 * r2, _.Item3))
        .Map(_ => new RulesCatalog(_.Item1, _.Item2));
}