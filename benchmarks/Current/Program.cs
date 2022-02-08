using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using RulesEngine;
using RulesEngine.Internals;
using RulesEngine.Models;
using Serilog.Core;

internal struct TestModel
{
    public string StringProperty { get; set; }
}

internal class Data
{
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
public class CurrentCompilerBenchmarks
{
    private readonly RulesManager<TestModel> _manager;

    public CurrentCompilerBenchmarks() => _manager = new RulesManager<TestModel>(new RulesCompiler(Logger.None));

    [Benchmark]
    public void SetCatalogBenchmark() => _manager.SetCatalog(Data.Catalog);
}

[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
internal class ApplierBenchmarks
{
    private readonly RulesManager<TestModel> _manager;
    private TestModel _item;

    public ApplierBenchmarks()
    {
        _manager = new RulesManager<TestModel>(new RulesCompiler(Logger.None));
        _item = new TestModel
        {
            StringProperty = "correct"
        };
    }

    [Benchmark]
    public void SetCatalogBenchmark() => _manager.SetCatalog(Data.Catalog);
}

internal static class Program
{
    internal static void Main()
    {
        BenchmarkRunner.Run<CurrentCompilerBenchmarks>();
    }
}