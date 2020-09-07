using Fabiolune.BusinessRulesEngine.Models;

namespace Fabiolune.BusinessRulesEngine.Interfaces
{
    public interface IBusinessRulesManager<in T> where T : new()
    {
        void SetCatalog(RulesCatalog catalog);
        RulesCatalogApplicationResult ItemSatisfiesRulesWithMessage(T item);
        bool ItemSatisfiesRules(T item);
    }
}