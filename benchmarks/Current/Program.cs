using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using LogicEngine;

namespace Current;

[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
public class CurrentImplementationBenchmarks
{
    private readonly RulesCatalogCompiler _compiler = new(new RulesSetCompiler(new SingleRuleCompiler()));

    private readonly Data.TestModel _item = new()
    {
        StringProperty = "correct"
    };

    private readonly RulesManager<Data.TestModel> _manager1 =
        new(new RulesCatalogCompiler(new RulesSetCompiler(new SingleRuleCompiler())));

    private readonly RulesManager<Data.TestModel> _manager2 =
        new(new RulesCatalogCompiler(new RulesSetCompiler(new SingleRuleCompiler())));

    public CurrentImplementationBenchmarks()
    {
        _manager1.Catalog = Data.ShortCircuitCatalog;
        _manager2.Catalog = Data.FullExecutingCatalog;
    }

    [Benchmark]
    public void SetCatalog() => _compiler.CompileCatalog<Data.TestModel>(Data.ShortCircuitCatalog);

    [Benchmark]
    public void RulesApplication_CircuitBreaking() => _manager1.ItemSatisfiesRules(_item);

    [Benchmark]
    public void RulesApplication_No_CircuitBreaking() => _manager2.ItemSatisfiesRules(_item);

    [Benchmark]
    public void ItemSatisfiesRulesWithMessage_CircuitBreaking() => _manager1.ItemSatisfiesRulesWithMessage(_item);

    [Benchmark]
    public void ItemSatisfiesRulesWithMessage_No_CircuitBreaking() =>
        _manager2.ItemSatisfiesRulesWithMessage(_item).OnLeft(_ =>
        {
            Console.Write(_.ElementAt(0));
        });
}

internal static class Program
{
    internal static void Main()
    {
        BenchmarkRunner.Run<CurrentImplementationBenchmarks>(DefaultConfig.Instance.WithOption(ConfigOptions.DisableOptimizationsValidator, true));
    }
}