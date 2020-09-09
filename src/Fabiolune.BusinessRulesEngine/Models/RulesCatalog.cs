using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Fabiolune.BusinessRulesEngine.Models
{
    public class RulesCatalog
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "ruleSets")]
        public IEnumerable<RuleSet> RuleSets { get; set; }

        // operators overload

        /// <summary>
        ///     This represents the logical OR between two catalogs
        /// </summary>
        /// <param name="catalog1"></param>
        /// <param name="catalog2"></param>
        /// <returns></returns>
        public static RulesCatalog operator +(RulesCatalog catalog1, RulesCatalog catalog2) 
            => new RulesCatalog
                {
                    Name = $"{catalog1.Name} OR {catalog2.Name}",
                    RuleSets = (catalog1.RuleSets ?? new List<RuleSet>()).Union(catalog2.RuleSets ?? new List<RuleSet>())
                };

        /// <summary>
        ///     his represents the logical AND between two catalogs
        /// </summary>
        /// <param name="catalog1"></param>
        /// <param name="catalog2"></param>
        /// <returns></returns>
        public static RulesCatalog operator *(RulesCatalog catalog1, RulesCatalog catalog2) 
            => new RulesCatalog
                {
                    Name = $"{catalog1.Name} AND {catalog2.Name}",
                    RuleSets = (from r1 in catalog1.RuleSets ?? new List<RuleSet>() from r2 in catalog2.RuleSets ?? new List<RuleSet>() select new RuleSet { Description = $"{r1.Description} AND {r2.Description}", Rules = (r1.Rules ?? new List<Rule>()).Union(r2.Rules ?? new List<Rule>()) }).ToList()
                };
    }
}