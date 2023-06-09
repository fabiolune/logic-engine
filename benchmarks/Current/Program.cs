//using BenchmarkDotNet.Attributes;
//using BenchmarkDotNet.Configs;
//using BenchmarkDotNet.Running;
//using LogicEngine;
//using LogicEngine.Compilers;
//using LogicEngine.Internals;
//using LogicEngine.Managers;
//using LogicEngine.Models;
//using TinyFp;
//using TinyFp.Extensions;

//namespace Current;

//[MemoryDiagnoser]
//[MarkdownExporterAttribute.GitHub]
//public class CurrentImplementationBenchmarks
//{
//    private readonly RulesCatalogCompiler _compiler = new(new RulesSetCompiler(new SingleRuleCompiler()));

//    private readonly Data.TestModel _item = new()
//    {
//        StringProperty = "correct"
//    };

//    private readonly RulesManager<Data.TestModel> _manager1 =
//        new(new RulesCatalogCompiler(new RulesSetCompiler(new SingleRuleCompiler())));

//    private readonly RulesManager<Data.TestModel> _manager2 =
//        new(new RulesCatalogCompiler(new RulesSetCompiler(new SingleRuleCompiler())));

//    public CurrentImplementationBenchmarks()
//    {
//        _manager1.Catalog = Data.ShortCircuitCatalog;
//        _manager2.Catalog = Data.FullExecutingCatalog;
//    }

//    [Benchmark]
//    public void SetCatalog() => _compiler.CompileCatalog<Data.TestModel>(Data.ShortCircuitCatalog);

//    [Benchmark]
//    public void RulesApplication_CircuitBreaking() => _manager1.ItemSatisfiesRules(_item);

//    [Benchmark]
//    public void RulesApplication_No_CircuitBreaking() => _manager2.ItemSatisfiesRules(_item);

//    [Benchmark]
//    public void ItemSatisfiesRulesWithMessage_CircuitBreaking() => _manager1.ItemSatisfiesRulesWithMessage(_item);

//    [Benchmark]
//    public void ItemSatisfiesRulesWithMessage_No_CircuitBreaking() =>
//        _manager2
//            .ItemSatisfiesRulesWithMessage(_item)
//            .OnLeft(_ => Console.WriteLine(_.Length));
//}
///// <summary>
///// Benchmarks in this class compare the execution of a
///// statically typed function versus one obtained by compiling a rule
///// </summary>
//[MemoryDiagnoser]
//public class FunctionBenchmarks
//{
//    private readonly Func<Data.TestModel, Either<string, Unit>> _compiledFunc;

//    public FunctionBenchmarks()
//    {
//        var rulescompiler = new SingleRuleCompiler();
//        var rule = new Rule(nameof(Data.TestModel.StringProperty), OperatorType.Equal, "0", "code");
//        _compiledFunc = rulescompiler.Compile<Data.TestModel>(rule).Unwrap().Executable;
//    }

//    private static Either<string, Unit> Check(Data.TestModel item)
//    {
//        return item.StringProperty == "0"
//            ? Either<string, Unit>.Right(Unit.Default)
//            : Either<string, Unit>.Left("code");
//    }

//    [Benchmark]
//    public void Compiled() => Executor(Check);

//    [Benchmark]
//    public void RuntimeCompiled() => Executor(_compiledFunc);

//    private static void Executor(Func<Data.TestModel, Either<string, Unit>> func) =>
//        Enumerable
//            .Range(0, 10000)
//            .Select(_ => new Data.TestModel
//            {
//                StringProperty = $"{_}"
//            })
//            .ForEach(_ => func(_).Match(_ => true, _ => false).Do(Console.WriteLine));

//}

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Current;
using LogicEngine;
using LogicEngine.Compilers;
using LogicEngine.Internals;
using LogicEngine.Models;
using TinyFp;

[MemoryDiagnoser]
public class CompileBenchmarks
{
    private static readonly Data.TestModel _input = new Data.TestModel { StringProperty = "a" };
    private static readonly RuleCompiler _compiler = new();
    private static readonly Rule _rule = new("StringProperty", OperatorType.Equal, "some", "code");
    private static readonly Func<Data.TestModel, Either<string, Unit>> _func = _ => Either<string, Unit>.Right(Unit.Default);
    private CompiledRule<Data.TestModel> _compiledRule = _compiler.Compile<Data.TestModel>(_rule).Unwrap();

    [Benchmark]
    public void CreateRule() => _compiler.Compile<Data.TestModel>(_rule);
}

internal static class Program
{
    internal static void Main()
    {
        BenchmarkRunner.Run<CompileBenchmarks>(DefaultConfig.Instance.WithOption(ConfigOptions.DisableOptimizationsValidator, true));
        //BenchmarkRunner.Run<CurrentImplementationBenchmarks>(DefaultConfig.Instance.WithOption(ConfigOptions.DisableOptimizationsValidator, true));
    }
}