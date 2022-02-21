using System;
using System.Collections.Generic;
using System.Linq;
using LogicEngine.Interfaces;
using LogicEngine.Models;
using TinyFp;
using TinyFp.Extensions;

namespace LogicEngine;

public class RulesSetManager<T> : IRulesSetManager<T> where T : new()
{
    private readonly IRulesSetCompiler _compiler;

    public RulesSet Set
    {
        set => _firstMatching =
            value
                .Map(_compiler.CompileLabeled<T>)
                .Executables
                .Match(AlwaysNoneFirstMatchingWithRules, () => AlwaysNoneFirstMatching);
    }

    public RulesSetManager(IRulesSetCompiler compiler) => _compiler = compiler;

    public Option<string> FirstMatching(T item) => _firstMatching(item);

    private static Func<T, Option<string>> AlwaysNoneFirstMatchingWithRules(List<KeyValuePair<string, Func<T, Either<string, Unit>>>> rules) => 
    item => rules
            .Select(x => (x.Key, x.Value(item)))
            .FirstOrDefault(x => x.Item2.IsRight)
            .ToOption(x => x.Equals(default))
        .Map(_ => _.Key);

    private Func<T, Option<string>> _firstMatching = AlwaysNoneFirstMatching;

    private static readonly Func<T, Option<string>> AlwaysNoneFirstMatching = _ => Option<string>.None();
}