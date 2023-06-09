using LogicEngine.Models;
using TinyFp;

namespace LogicEngine.Interfaces.Compilers;

public interface IRulesSetCompiler
{
    Option<CompiledRulesSet<T>> Compile<T>(RulesSet set) where T : new();
}