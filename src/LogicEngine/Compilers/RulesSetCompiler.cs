using LogicEngine.Interfaces.Compilers;
using LogicEngine.Models;
using System.Linq;
using TinyFp;
using TinyFp.Extensions;

namespace LogicEngine.Compilers;

public class RulesSetCompiler(IRuleCompiler singleRuleCompiler) : IRulesSetCompiler
{
    private readonly IRuleCompiler _singleRuleCompiler = singleRuleCompiler;

    /// <summary>
    /// Compiles a set of rules into a compiled rules set for the specified type.
    /// </summary>
    /// <remarks>This method processes the rules in the provided <paramref name="set"/> by compiling each rule
    /// individually, filtering out invalid or uncompiled rules, and then aggregating the results into a compiled rules
    /// set.</remarks>
    /// <typeparam name="T">The type of object the rules will operate on. Must have a parameterless constructor.</typeparam>
    /// <param name="set">The set of rules to compile. Cannot be null.</param>
    /// <returns>An <see cref="Option{T}"/> containing a <see cref="CompiledRulesSet{T}"/> if the compilation is successful, or
    /// an empty option if no rules are compiled.</returns>
    public Option<CompiledRulesSet<T>> Compile<T>(RulesSet set) where T : new() =>
        set
            .Rules
            .Select(_singleRuleCompiler.Compile<T>)
            .Filter()
            .ToArray()
            .ToOption(e => e.Length == 0)
            .Map(rules => new CompiledRulesSet<T>(rules, set.Name));
}