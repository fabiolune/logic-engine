using System.Linq;
using LogicEngine.Interfaces.Compilers;
using LogicEngine.Models;
using TinyFp;
using TinyFp.Extensions;

namespace LogicEngine.Compilers;

public class RulesCatalogCompiler : IRulesCatalogCompiler
{
    private readonly IRulesSetCompiler _rulesSetCompiler;

    public RulesCatalogCompiler(IRulesSetCompiler rulesSetCompiler) => _rulesSetCompiler = rulesSetCompiler;

    public Option<CompiledCatalog<T>> Compile<T>(RulesCatalog catalog) where T : new() =>
        catalog.RulesSets
            .Where(rs => rs is not null)
            .Select(_rulesSetCompiler.Compile<T>)
            .Filter()
            .ToArray()
            .ToOption(e => !e.Any())
            .Map(_ => new CompiledCatalog<T>(_, catalog.Name));
}