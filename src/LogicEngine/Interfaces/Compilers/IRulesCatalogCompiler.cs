using LogicEngine.Models;
using TinyFp;

namespace LogicEngine.Interfaces.Compilers;

public interface IRulesCatalogCompiler
{
    Option<CompiledCatalog<T>> Compile<T>(RulesCatalog catalog) where T : new();
}