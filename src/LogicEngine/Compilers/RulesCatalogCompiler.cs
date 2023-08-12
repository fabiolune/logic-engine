using LogicEngine.Interfaces.Compilers;
using LogicEngine.Models;
using System.Linq;
using TinyFp;
using TinyFp.Extensions;

namespace LogicEngine.Compilers;

public class RulesCatalogCompiler : IRulesCatalogCompiler
{
    private readonly IRulesSetCompiler _rulesSetCompiler;

    public RulesCatalogCompiler(IRulesSetCompiler rulesSetCompiler) => _rulesSetCompiler = rulesSetCompiler;

    /// <summary>
    /// The function compiles a catalog of rules into a compiled catalog of a specified type, returning
    /// an option that contains the compiled catalog if successful.
    /// </summary>
    /// <param name="RulesCatalog">The `RulesCatalog` parameter is an object that contains a collection
    /// of `RulesSets`. Each `RulesSet` represents a set of rules that can be compiled into a
    /// `CompiledCatalog<T>`. The `Compile<T>` method takes this `RulesCatalog` as input and returns an
    /// `Option<</param>
    public Option<CompiledCatalog<T>> Compile<T>(RulesCatalog catalog) where T : new() =>
        catalog.RulesSets
            .Where(rs => rs is not null)
            .Select(_rulesSetCompiler.Compile<T>)
            .Filter()
            .ToArray()
            .ToOption(e => !e.Any())
            .Map(_ => new CompiledCatalog<T>(_, catalog.Name));
}