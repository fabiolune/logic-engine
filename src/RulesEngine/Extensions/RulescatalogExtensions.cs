using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RulesEngine.Models;

namespace RulesEngine.Extensions
{
    public static class RulesCatalogExtensions
    {
        public static IEnumerable<IEnumerable<Rule>> ToSimplifiedCatalog(this RulesCatalog catalog)
            => catalog?.RulesSets == null 
                    ? Array.Empty<IEnumerable<Rule>>() 
                    : catalog.RulesSets.Select(rs => rs.Rules);

        public static bool IsEquivalentTo(this RulesCatalog catalog, RulesCatalog comparedCatalog)
            => JsonConvert
                .SerializeObject(catalog.ToSimplifiedCatalog())
                .Equals(JsonConvert.SerializeObject(comparedCatalog.ToSimplifiedCatalog()));
    }
}