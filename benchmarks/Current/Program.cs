using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using LogicEngine;
using Serilog.Core;

namespace Current;

[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
public class CurrentImplementationBenchmarks
{
    private readonly RulesManager<Data.TestModel> _manager0 = new(new RulesCompiler(Logger.None));
    private readonly RulesManager<Data.TestModel> _manager1 = new(new RulesCompiler(Logger.None));
    private readonly RulesManager<Data.TestModel> _manager2 = new(new RulesCompiler(Logger.None));

    private readonly Data.TestModel _item = new()
    {
        StringProperty = "correct"
    };

    public CurrentImplementationBenchmarks()
    {
        _manager1.SetCatalog(Data.ShortCircuitCatalog);
        _manager2.SetCatalog(Data.FullExecutingCatalog);
    }

    [Benchmark]
    public void SetCatalog() => _manager0.SetCatalog(Data.ShortCircuitCatalog);

    [Benchmark]
    public void RulesApplication_CircuitBreaking() => _manager1.ItemSatisfiesRules(_item);

    [Benchmark]
    public void RulesApplication_No_CircuitBreaking() => _manager2.ItemSatisfiesRules(_item);

    [Benchmark]
    public void ItemSatisfiesRulesWithMessage_CircuitBreaking() => _manager1.ItemSatisfiesRulesWithMessage(_item);

    [Benchmark]
    public void ItemSatisfiesRulesWithMessage_No_CircuitBreaking() => _manager2.ItemSatisfiesRulesWithMessage(_item);
}

internal static class Program
{
    internal static void Main()
    {
        BenchmarkRunner.Run<CurrentImplementationBenchmarks>(DefaultConfig.Instance.WithOption(ConfigOptions.DisableOptimizationsValidator, true));
    }
}