using System.Collections.Generic;
using RulesEngine.Models;

namespace RulesEngine.Interfaces
{
    public interface IRulesManager<T> where T : new()
    {
        void SetCatalog(RulesCatalog catalog);
        RulesCatalogApplicationResult ItemSatisfiesRulesWithMessage(T item);
        bool ItemSatisfiesRules(T item);
        IEnumerable<T> Filter(IEnumerable<T> items);
        T FirstOrDefault(IEnumerable<T> items);
    }
}