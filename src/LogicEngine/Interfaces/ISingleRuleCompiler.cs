using LogicEngine.Models;
using TinyFp;

namespace LogicEngine.Interfaces;

public interface ISingleRuleCompiler
{
    Option<CompiledRule<T>> Compile<T>(Rule rule);
}