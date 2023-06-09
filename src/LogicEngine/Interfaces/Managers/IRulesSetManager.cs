using LogicEngine.Interfaces.Compilers;
using LogicEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using TinyFp;
using TinyFp.Extensions;

namespace LogicEngine.Interfaces.Managers;

public interface IRulesSetManager<T> where T : new()
{

    //RulesSet Set { set; }
    Option<string> FirstMatching(T item);
    IEnumerable<T> Filter(IEnumerable<T> items);
    //bool ItemSatisfiesRules(T item);
    //Either<string[], Unit> ItemSatisfiesRulesWithMessage(T item);
}

public record RulesSetManager<T> : IRulesSetManager<T> where T : new()
{
    private readonly CompiledRulesSet<T> _rulesSet;

    public RulesSetManager(CompiledRulesSet<T> rulesSet) => _rulesSet = rulesSet;

    public IEnumerable<T> Filter(IEnumerable<T> items) => items.Where(_rulesSet.Apply);

    public Option<string> FirstMatching(T item)
    {
        throw new NotImplementedException();
    }
}
