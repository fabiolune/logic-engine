using System.Collections.Generic;
using RulesEngine.Models;
using TinyFp;

namespace RulesEngine.Interfaces
{
    public interface IRulesManager<T> where T : new()
    {
        void SetCatalog(RulesCatalog catalog);
        Either<IEnumerable<Option<string>>, Unit> ItemSatisfiesRulesWithMessage(T item);
        bool ItemSatisfiesRules(T item);
        IEnumerable<T> Filter(IEnumerable<T> items);
        T FirstOrDefault(IEnumerable<T> items);
    }
}