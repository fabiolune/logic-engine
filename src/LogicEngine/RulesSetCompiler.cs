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
            .AsParallel()
            .Select(_ => new KeyValuePair<string, Option<CompiledRule<T>>>(_.Code ?? string.Empty, _singleRuleCompiler.Compile<T>(_)))
            .Where(_ => _.Value.IsSome)
            .Select(_ =>
                new KeyValuePair<string, Func<T, Either<string, Unit>>>(_.Key, _.Value.Unwrap().Executable))
            .ToList()
            .ToOption(_ => _.Count == 0)
            .Map(_ => new CompiledLabeledRulesSet<T>(_));
}