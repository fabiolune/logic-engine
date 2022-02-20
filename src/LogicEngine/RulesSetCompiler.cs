using System.Linq;
using LogicEngine.Interfaces;
using LogicEngine.Models;
using TinyFp.Extensions;

namespace LogicEngine;

public class RulesSetCompiler : IRulesSetCompiler
{
    private readonly ISingleRuleCompiler _singleRuleCompiler;

    public RulesSetCompiler(ISingleRuleCompiler singleRuleCompiler) => _singleRuleCompiler = singleRuleCompiler;

    public CompiledRulesSet<T> Compile<T>(RulesSet set) =>
        set
            .Rules
            .AsParallel()
            .Select(_singleRuleCompiler.Compile<T>)
            .Where(_ => _.IsSome)
            .Select(_ => _.Unwrap())
            .Select(_ => _.Executable)
            .ToArray()
            .Map(_ => new CompiledRulesSet<T>(_));
}