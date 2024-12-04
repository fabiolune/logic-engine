using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace LogicEngine.Models;

public record RulesSet
{
    private IEnumerable<Rule> _rules;
    public string Name { get; }
    /// <summary>
    /// Creates a new rule set
    /// </summary>
    /// <param name="rules"></param>
    /// <param name="name"></param>
    public RulesSet(IEnumerable<Rule> rules, string name) => (_rules, Name) = (rules, name);
    /// <summary>
    /// Rules that make up the set
    /// </summary>
    [DataMember(Name = "rules")]
    public IEnumerable<Rule> Rules { get => _rules ?? []; init => _rules = value; }
    /// <summary>
    /// Combines two rule sets into one
    /// </summary>
    /// <param name="set1"></param>
    /// <param name="set2"></param>
    /// <returns></returns>
    public static RulesSet operator *(RulesSet set1, RulesSet set2) =>
        new(set1.Rules.Concat(set2.Rules), $"({set1.Name} AND {set2.Name})");
}