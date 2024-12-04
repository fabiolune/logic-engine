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
    /// ompiles a RulesCatalog into a <see cref="CompiledCatalog{T}"/>.
    /// It filters out any null RulesSets, compiles each valid RulesSet using the <see cref="IRulesSetCompiler"/>, and then constructs a <see cref="CompiledCatalog{T}"/> with the compiled rule sets and the catalog's name.
    /// The result is wrapped in an Option to handle cases where no valid rule sets are present.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="catalog"></param>
    /// <returns></returns>
    public Option<CompiledCatalog<T>> Compile<T>(RulesCatalog catalog) where T : new() =>
        catalog.RulesSets
            .Where(rs => rs is not null)
            .Select(_rulesSetCompiler.Compile<T>)
            .Filter()
            .ToArray()
            .ToOption(e => e.Length == 0)
            .Map(set => new CompiledCatalog<T>(set, catalog.Name));
}