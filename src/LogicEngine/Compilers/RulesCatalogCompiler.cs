using LogicEngine.Interfaces.Compilers;
using LogicEngine.Models;
using System.Linq;
using TinyFp;
using TinyFp.Extensions;

namespace LogicEngine.Compilers;

public class RulesCatalogCompiler(IRulesSetCompiler rulesSetCompiler) : IRulesCatalogCompiler
{
    private readonly IRulesSetCompiler _rulesSetCompiler = rulesSetCompiler;

    /// <summary>
    /// Compiles a rules catalog into a compiled catalog for the specified type.
    /// </summary>
    /// <remarks>This method filters and compiles the rule sets in the provided catalog, producing a compiled
    /// catalog that can be used for rule evaluation. If no valid rule sets are found, the method returns an empty
    /// option.</remarks>
    /// <typeparam name="T">The type of the objects that the compiled catalog will operate on. Must have a parameterless constructor.</typeparam>
    /// <param name="catalog">The rules catalog to compile. Must not be null and must contain valid rule sets.</param>
    /// <returns>An <see cref="Option{T}"/> containing a <see cref="CompiledCatalog{T}"/> if the compilation is successful;
    /// otherwise, an empty option if no valid rule sets are found.</returns>
    public Option<CompiledCatalog<T>> Compile<T>(RulesCatalog catalog) where T : new() =>
        catalog.RulesSets
            .Where(rs => rs is not null)
            .Select(_rulesSetCompiler.Compile<T>)
            .Filter()
            .ToArray()
            .ToOption(e => e.Length == 0)
            .Map(set => new CompiledCatalog<T>(set, catalog.Name));
}