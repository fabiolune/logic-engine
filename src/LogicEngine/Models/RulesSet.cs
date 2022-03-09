using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LogicEngine.Models;

public struct RulesSet
{
    private IEnumerable<Rule> _rules;

    [DataMember(Name = "rules")]
    public IEnumerable<Rule> Rules { get => _rules ?? Array.Empty<Rule>(); set => _rules = value; }
}