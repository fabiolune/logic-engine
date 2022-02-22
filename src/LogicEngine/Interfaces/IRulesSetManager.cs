using LogicEngine.Models;
using TinyFp;

namespace LogicEngine.Interfaces;

public interface IRulesSetManager<in T> where T : new()
{
    RulesSet Set { set; }
    Option<string> FirstMatching(T item);
}