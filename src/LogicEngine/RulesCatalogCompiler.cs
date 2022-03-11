using System.Linq;
using LogicEngine.Interfaces;
using LogicEngine.Models;
using TinyFp.Extensions;

namespace LogicEngine;

public class RulesCatalogCompiler : IRulesCatalogCompiler
{
    private readonly IRulesSetCompiler _rulesSetCompiler;

    public RulesCatalogCompiler(IRulesSetCompiler rulesSetCompiler) => _rulesSetCompiler = rulesSetCompiler;

    public CompiledCatalog<T> CompileCatalog<T>(RulesCatalog catalog) =>
        catalog.RulesSets
            .AsParallel()
            .Select(_rulesSetCompiler.Compile<T>)
            .Select(_ => _.Executables)
            .ToArray()
            .Map(_ => new CompiledCatalog<T>(_));
}