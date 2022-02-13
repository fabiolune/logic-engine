using System;
using System.Collections.Generic;
using System.Linq;
using LogicEngine.Models;
using Newtonsoft.Json;
using TinyFp.Extensions;

namespace LogicEngine.Extensions
{
    public static class RulesCatalogExtensions
    {
        public static IEnumerable<IEnumerable<Rule>> ToSimplifiedCatalog(this RulesCatalog catalog)
            => catalog
                .ToOption(_ => _.RulesSets == null)
                .Match(_ => _.RulesSets.Select(rs => rs.Rules), Array.Empty<IEnumerable<Rule>>);

        public static bool IsEquivalentTo(this RulesCatalog catalog, RulesCatalog comparedCatalog)
            => JsonConvert
                .SerializeObject(catalog.ToSimplifiedCatalog())
                .Equals(JsonConvert.SerializeObject(comparedCatalog.ToSimplifiedCatalog()));
    }
}