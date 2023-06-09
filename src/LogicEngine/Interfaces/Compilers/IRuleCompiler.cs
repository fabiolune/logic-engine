using LogicEngine.Models;
using TinyFp;

namespace LogicEngine.Interfaces.Compilers;

public interface IRuleCompiler
{
    Option<CompiledRule<T>> Compile<T>(Rule rule) where T : new();
}