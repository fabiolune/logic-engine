using System.Collections.Generic;
using LogicEngine.Models;
using TinyFp;

namespace LogicEngine.Interfaces.Managers;

public interface IRulesCatalogManager<T> where T : new()
{
    RulesCatalog Catalog { set; }
    Either<string[], Unit> ItemSatisfiesRulesWithMessage(T item);
    bool ItemSatisfiesRules(T item);
    IEnumerable<T> Filter(IEnumerable<T> items);
    T FirstOrDefault(IEnumerable<T> items);
}