using BenchmarkDotNet.Attributes;
using LogicEngine;
using LogicEngine.Compilers;

namespace Current;

[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
public class ApplyBenchmarks
{
    private readonly RulesCatalogCompiler _compiler = new(new RulesSetCompiler(new RuleCompiler()));

    private readonly Data.TestModel _item = new()
    {
        StringProperty = "correct"
    };

    private readonly CompiledCatalog<Data.TestModel> _compiledCatalog1;
    private readonly CompiledCatalog<Data.TestModel> _compiledCatalog2;

    public ApplyBenchmarks()
    {
        _compiledCatalog1 = _compiler.Compile<Data.TestModel>(Data.ShortCircuitCatalog).Unwrap();
        _compiledCatalog2 = _compiler.Compile<Data.TestModel>(Data.FullExecutingCatalog).Unwrap();
    }

    [Benchmark]
    public void Apply1() => _compiledCatalog1.Apply(_item);

    [Benchmark]
    public void Apply2() => _compiledCatalog2.Apply(_item);
}
