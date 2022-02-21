using LogicEngine.Models;

namespace LogicEngine.Interfaces;

public interface IRulesSetCompiler
{
    CompiledRulesSet<T> Compile<T>(RulesSet set);
    CompiledLabeledRulesSet<T> CompileLabeled<T>(RulesSet set);
}