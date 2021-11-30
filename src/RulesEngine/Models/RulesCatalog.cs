using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace RulesEngine.Models
{
    public class RulesCatalog
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "ruleSets")]
        public IEnumerable<RulesSet> RulesSets { get; set; }

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
                    RulesSets = (catalog1.RulesSets ?? new List<RulesSet>()).Union(catalog2.RulesSets ?? new List<RulesSet>())
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
                    RulesSets = (from r1 in catalog1.RulesSets ?? new List<RulesSet>() from r2 in catalog2.RulesSets ?? new List<RulesSet>() select new RulesSet { Description = $"{r1.Description} AND {r2.Description}", Rules = (r1.Rules ?? new List<Rule>()).Union(r2.Rules ?? new List<Rule>()) }).ToList()
                };
    }
}