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
[JsonExporterAttribute.Brief]
[MarkdownExporterAttribute.GitHub]
public class CurrentCompilerBenchmarks
{
    private readonly RulesManager<Data.TestModel> _manager = new(new RulesCompiler(Logger.None));

    [Benchmark]
    public void SetCatalogBenchmark() => _manager.SetCatalog(Data.Catalog);
}

[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
[JsonExporterAttribute.Brief]
public class CurrentApplierBenchmarks
{
    private readonly RulesManager<Data.TestModel> _manager = new(new RulesCompiler(Logger.None));

    private readonly Data.TestModel _item = new()
    {
        StringProperty = "correct"
    };

    public CurrentApplierBenchmarks()
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
        BenchmarkRunner.Run<CurrentCompilerBenchmarks>();
        BenchmarkRunner.Run<CurrentApplierBenchmarks>();
    }
}