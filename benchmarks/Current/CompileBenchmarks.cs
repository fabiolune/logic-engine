using BenchmarkDotNet.Attributes;
using LogicEngine.Compilers;

namespace Current;

[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
public class CompileBenchmarks
{
    private readonly RulesCatalogCompiler _compiler = new(new RulesSetCompiler(new RuleCompiler()));

    [Benchmark]
    public void Compile1() => _compiler.Compile<Data.TestModel>(Data.ShortCircuitCatalog);

    [Benchmark]
    public void Compile2() => _compiler.Compile<Data.TestModel>(Data.FullExecutingCatalog);

}
