using LogicEngine.Models;
using System.Collections.Generic;
using TinyFp;

namespace LogicEngine.Interfaces;

public interface ICompositeRulesSetManager<in T, TKey> where T : new()
{
    IEnumerable<(TKey,RulesSet)> Set { set; }
    Option<TKey> FirstMatching(T item);
}
