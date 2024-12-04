using LogicEngine.Models;
using TinyFp;

namespace LogicEngine.Interfaces.Compilers;

public interface IRuleCompiler
{
    /// <summary>
    /// Compiles a rule into a compiled rule, None if the rule is invalid
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="rule"></param>
    /// <returns></returns>
    Option<CompiledRule<T>> Compile<T>(Rule rule) where T : new();
}