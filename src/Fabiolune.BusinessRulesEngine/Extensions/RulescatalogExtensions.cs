using System;
using System.Collections.Generic;
using System.Linq;
using Fabiolune.BusinessRulesEngine.Models;
using Newtonsoft.Json;

namespace Fabiolune.BusinessRulesEngine.Extensions
{
    public static class RulesCatalogExtensions
    {
        public static IEnumerable<IEnumerable<Rule>> ToSimplifiedCatalog(this RulesCatalog catalog)
            => catalog?.RuleSets == null 
                    ? Array.Empty<IEnumerable<Rule>>() 
                    : catalog.RuleSets.Select(rs => rs.Rules);

        public static bool IsEquivalentTo(this RulesCatalog catalog, RulesCatalog comparedCatalog)
            => JsonConvert
                .SerializeObject(catalog.ToSimplifiedCatalog())
                .Equals(JsonConvert.SerializeObject(comparedCatalog.ToSimplifiedCatalog()));
    }
}