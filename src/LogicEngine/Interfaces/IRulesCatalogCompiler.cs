using LogicEngine.Models;

namespace LogicEngine.Interfaces;

public interface IRulesCatalogCompiler
{
    public CompiledCatalog<T> CompileCatalog<T>(RulesCatalog catalog);
}