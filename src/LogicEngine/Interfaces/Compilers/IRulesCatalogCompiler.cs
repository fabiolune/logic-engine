using LogicEngine.Models;
using TinyFp;

namespace LogicEngine.Interfaces.Compilers;

public interface IRulesCatalogCompiler
{
    /// <summary>
    /// Compiles a catalog of rules into a compiled catalog, None if the catalog is invalid
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="catalog"></param>
    /// <returns></returns>
    Option<CompiledCatalog<T>> Compile<T>(RulesCatalog catalog) where T : new();
}