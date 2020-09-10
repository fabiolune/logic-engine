using Fabiolune.BusinessRulesEngine.Interfaces;
using Fabiolune.BusinessRulesEngine.Models;

namespace Fabiolune.BusinessRulesEngine.Extensions
{
    public static class Extensions
    {
        public static bool SatisfiesRules<T>(this T @this, IBusinessRulesManager<T> manager) where T : new() =>
            manager.ItemSatisfiesRules(@this);

        public static RulesCatalogApplicationResult SatisfiesRulesWithMessage<T>(this T @this, IBusinessRulesManager<T> manager) where T : new() =>
            manager.ItemSatisfiesRulesWithMessage(@this);
    }
}