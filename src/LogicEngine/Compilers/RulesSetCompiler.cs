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
    /// Compiles a given RulesSet into an Option<<see cref="CompiledRulesSet{T}"/>>. It does this by compiling each individual rule in the set using the <see cref="IRuleCompiler"/>, filtering out any invalid rules, and then creating a new <see cref="CompiledRulesSet{T}"/> with the valid compiled rules and the original set's name. If no rules are valid, it returns None.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="set"></param>
    /// <returns></returns>
    public Option<CompiledRulesSet<T>> Compile<T>(RulesSet set) where T : new() =>
        set
            .Rules
            .Select(_singleRuleCompiler.Compile<T>)
            .Filter()
            .ToArray()
            .ToOption(e => e.Length == 0)
            .Map(rules => new CompiledRulesSet<T>(rules, set.Name));
}