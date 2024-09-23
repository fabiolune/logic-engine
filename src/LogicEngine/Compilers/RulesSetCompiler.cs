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

    /// <summary>
    /// The function compiles a set of rules into a compiled rules set for a given type, returning an
    /// option type.
    /// </summary>
    /// <param name="RulesSet">The `RulesSet` parameter is an object that represents a set of rules. It
    /// contains a collection of rules that need to be compiled.</param>
    public Option<CompiledRulesSet<T>> Compile<T>(RulesSet set) where T : new() =>
        set
            .Rules
            .Select(_singleRuleCompiler.Compile<T>)
            .Filter()
            .ToArray()
            .ToOption(e => e.Length == 0)
            .Map(rules => new CompiledRulesSet<T>(rules, set.Name));
}