using System;
using System.Collections.Generic;
using System.Linq;
using LogicEngine.Interfaces;
using LogicEngine.Models;
using TinyFp;
using TinyFp.Extensions;

namespace LogicEngine;

public class RulesSetCompiler : IRulesSetCompiler
{
    private readonly ISingleRuleCompiler _singleRuleCompiler;

    public RulesSetCompiler(ISingleRuleCompiler singleRuleCompiler) => _singleRuleCompiler = singleRuleCompiler;

    public CompiledRulesSet<T> Compile<T>(RulesSet set) =>
        (set.Rules ?? Array.Empty<Rule>())
            .AsParallel()
            .Select(_singleRuleCompiler.Compile<T>)
            .Where(_ => _.IsSome)
            .Select(_ => _.Unwrap())
            .Select(_ => _.Executable)
            .ToArray()
            .Map(_ => new CompiledRulesSet<T>(_));

    public CompiledLabeledRulesSet<T> CompileLabeled<T>(RulesSet set) =>
        (set.Rules ?? Array.Empty<Rule>())
            .ToOption(_ => !_.Any())
            .Map(_ => _.AsParallel()
            
                .Select(x => new KeyValuePair<string, Option<CompiledRule<T>>>(x.Code ?? string.Empty, _singleRuleCompiler.Compile<T>(x)))
                .Where(x => x.Value.IsSome)
                .Select(x =>
                    new KeyValuePair<string, Func<T, Either<string, Unit>>>(x.Key, x.Value.Unwrap().Executable))
                .ToArray())
            .Bind(_ => _.ToOption(x => x.Length == 0))
            .Map(_ => new CompiledLabeledRulesSet<T>(_));
    
}