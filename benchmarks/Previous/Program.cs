using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using RulesEngine;
using RulesEngine.Internals;
using RulesEngine.Models;
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
public class PreviousCompilerBenchmarks
{
    private readonly RulesManager<Data.TestModel> _manager = new(new RulesCompiler(Logger.None));

    [Benchmark]
    public void SetCatalogBenchmark() => _manager.SetCatalog(Data.Catalog);
}

[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
[JsonExporterAttribute.Brief]
public class PreviousApplierBenchmarks
{
    private readonly RulesManager<Data.TestModel> _manager = new(new RulesCompiler(Logger.None));

    private readonly Data.TestModel _item = new()
    {
        StringProperty = "correct"
    };

    public PreviousApplierBenchmarks()
    {
        _manager.SetCatalog(Data.Catalog);
    }

    [Benchmark]
    public void RuleApplicationBenchmark() => _manager.ItemSatisfiesRules(_item);
}

internal static class Program
{
    internal static void Main()
    {
        BenchmarkRunner.Run<PreviousCompilerBenchmarks>();
        BenchmarkRunner.Run<PreviousApplierBenchmarks>();
    }
}