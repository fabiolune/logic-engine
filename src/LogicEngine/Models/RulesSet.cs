using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;

namespace LogicEngine.Models;

public record RulesSet
{
    private IEnumerable<Rule> _rules;
    public string Name { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="RulesSet"/> class with the specified rules and name.
    /// </summary>
    /// <param name="rules">A collection of <see cref="Rule"/> objects that define the rules for this set. Cannot be null.</param>
    /// <param name="name">The name of the rules set. Cannot be null or empty.</param>
    public RulesSet(IEnumerable<Rule> rules, string name) => (_rules, Name) = (rules, name);

    /// <summary>
    /// Rules that make up the set
    /// </summary>
    [DataMember(Name = "rules")]
    [Required]
    public IEnumerable<Rule> Rules { get => _rules ?? []; init => _rules = value; }
    
    /// <summary>
    /// Combines two <see cref="RulesSet"/> instances into a new <see cref="RulesSet"/>  that contains the concatenated
    /// rules and a combined name.
    /// </summary>
    /// <param name="set1">The first <see cref="RulesSet"/> to combine.</param>
    /// <param name="set2">The second <see cref="RulesSet"/> to combine.</param>
    /// <returns>A new <see cref="RulesSet"/> that contains the rules from both <paramref name="set1"/>  and <paramref
    /// name="set2"/>, and a name representing their logical combination.</returns>
    public static RulesSet operator *(RulesSet set1, RulesSet set2) =>
        new(set1.Rules.Concat(set2.Rules), $"({set1.Name} AND {set2.Name})");
}