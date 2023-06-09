using LogicEngine.Interfaces.Compilers;
using LogicEngine.Models;
using System.Linq;
using TinyFp;
using TinyFp.Extensions;

namespace LogicEngine.Compilers;

public class RulesSetCompiler : IRulesSetCompiler
{
    private readonly IRuleCompiler _singleRuleCompiler;

    public RulesSetCompiler(IRuleCompiler singleRuleCompiler) => _singleRuleCompiler = singleRuleCompiler;

    public Option<CompiledRulesSet<T>> Compile<T>(RulesSet set) where T : new() =>
        set
            .Rules
            .AsParallel()
            .Select(_singleRuleCompiler.Compile<T>)
            .Filter()
            .ToOption(e => !e.Any())
            .Map(_ => new CompiledRulesSet<T>(_.ToArray()));
}