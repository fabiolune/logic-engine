using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace LogicEngine.Models;

public record RulesSet
{
    private IEnumerable<Rule> _rules;
    public string Name { get; }
    public RulesSet(IEnumerable<Rule> rules, string name) => (_rules, Name) = (rules, name);

    [DataMember(Name = "rules")]
    public IEnumerable<Rule> Rules { get => _rules ?? Array.Empty<Rule>(); init => _rules = value; }

    public static RulesSet operator *(RulesSet set1, RulesSet set2) =>
        new(set1.Rules.Concat(set2.Rules), $"({set1.Name} AND {set2.Name})");
}