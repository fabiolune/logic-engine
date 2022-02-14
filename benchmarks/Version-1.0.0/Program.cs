using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using LogicEngine;
using LogicEngine.Internals;
using LogicEngine.Models;
using Serilog.Core;

internal class Data
{
    internal struct TestModel
    {
        public string StringProperty { get; set; }
    }
    internal static RulesCatalog Catalog =>
    new()
    {
        RulesSets = new List<RulesSet>
                {
                    new()
                    {
                        Rules = new List<Rule>
                        {
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        }
                    },
                    new()
                    {
                        Rules = new List<Rule>
                        {
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                            new(nameof(TestModel.StringProperty), OperatorType.Equal, "correct"),
                        }
                    }
                }
    };
}

[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
[JsonExporterAttribute.Brief]
public class PreviousImplementationBenchmarks
{
    private readonly RulesManager<Data.TestModel> _manager = new(new RulesCompiler(Logger.None));
    private readonly RulesManager<Data.TestModel> _manager2 = new(new RulesCompiler(Logger.None));

    private readonly Data.TestModel _item = new()
    {
        StringProperty = "correct"
    };

    public PreviousImplementationBenchmarks()
    {
        _manager.SetCatalog(Data.Catalog);
    }

    [Benchmark]
    public void SetCatalog() => _manager2.SetCatalog(Data.Catalog);

    [Benchmark]
    public void RulesApplication() => _manager.ItemSatisfiesRules(_item);

    [Benchmark]
    public void RulesApplicationWithMessage() => _manager.ItemSatisfiesRulesWithMessage(_item);
}

internal static class Program
{
    internal static void Main()
    {
        BenchmarkRunner.Run<PreviousImplementationBenchmarks>(DefaultConfig.Instance.WithOption(ConfigOptions.DisableOptimizationsValidator, true));
    }
}