using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Fabiolune.BusinessRulesEngine.Models
{
    public class RuleSet
    {
        [DataMember(Name = "description")]
        public string Description { get; set; }
        [DataMember(Name = "rules")]
        public IEnumerable<Rule> Rules { get; set; }
    }
}