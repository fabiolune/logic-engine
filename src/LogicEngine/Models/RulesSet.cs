using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LogicEngine.Models;

public class RulesSet
{
    [DataMember(Name = "rules")]
    public IEnumerable<Rule> Rules { get; set; }
}