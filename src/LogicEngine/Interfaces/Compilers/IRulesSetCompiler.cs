using LogicEngine.Models;
using TinyFp;

namespace LogicEngine.Interfaces.Compilers;

public interface IRulesSetCompiler
{
    /// <summary>
    /// Compiles a rule set into a compiled rule set, None if the rule set is invalid
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="set"></param>
    /// <returns></returns>
    Option<CompiledRulesSet<T>> Compile<T>(RulesSet set) where T : new();
}