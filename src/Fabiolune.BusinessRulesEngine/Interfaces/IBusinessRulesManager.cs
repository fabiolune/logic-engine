using System.Collections.Generic;
using Fabiolune.BusinessRulesEngine.Models;

namespace Fabiolune.BusinessRulesEngine.Interfaces
{
    public interface IBusinessRulesManager<T> where T : new()
    {
        void SetCatalog(RulesCatalog catalog);
        RulesCatalogApplicationResult ItemSatisfiesRulesWithMessage(T item);
        bool ItemSatisfiesRules(T item);
        IEnumerable<T> Filter(IEnumerable<T> items);
    }
}