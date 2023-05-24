using LogicEngine.Interfaces;
using LogicEngine.Models;
using System.Collections.Generic;
using System.Linq;
using TinyFp;
using TinyFp.Extensions;

namespace LogicEngine;

public class CompositeRulesSetManager<T, TKey> : ICompositeRulesSetManager<T, TKey> where T : new()
{
    private readonly IRulesSetCompiler _compiler;

    private IEnumerable<(TKey, RulesSet)> _data;

    public CompositeRulesSetManager(IRulesSetCompiler compiler) => _compiler = compiler;

    public IEnumerable<(TKey, RulesSet)> Set { set => _data = value; }

    public Option<TKey> FirstMatching(T item) => 
        _data
            .Map(t => (t.Item1, _compiler.Compile<T>(t.Item2)))
            .Map(t => (t.Item1, t.Item2.Executables))
            .FirstOrDefault(t => t.Executables.Any(f => f(item).IsRight))
            .ToOption(t => t.Item1 is null)
            .Map(t => t.Item1);
}
