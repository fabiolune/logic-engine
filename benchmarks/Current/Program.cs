using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using LogicEngine;
using LogicEngine.Compilers;

namespace Current;

internal static class Program
{
    internal static void Main()
    {
        BenchmarkRunner.Run<CurrentImplementationBenchmarks>(DefaultConfig.Instance.WithOption(ConfigOptions.DisableOptimizationsValidator, true));
    }
}

[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
public class CurrentImplementationBenchmarks
{
    private readonly RulesCatalogCompiler _compiler = new(new RulesSetCompiler(new RuleCompiler()));

    private readonly Data.TestModel _item = new()
    {
        StringProperty = "correct"
    };

    private readonly CompiledCatalog<Data.TestModel> _compiledCatalog1;
    private readonly CompiledCatalog<Data.TestModel> _compiledCatalog2;

    //private readonly RulesManager<Data.TestModel> _manager2 =
    //    new(new RulesCatalogCompiler(new RulesSetCompiler(new SingleRuleCompiler())));

    public CurrentImplementationBenchmarks()
    {
        _compiledCatalog1 = _compiler.Compile<Data.TestModel>(Data.ShortCircuitCatalog).Unwrap();
        _compiledCatalog2 = _compiler.Compile<Data.TestModel>(Data.FullExecutingCatalog).Unwrap();
        //_manager1.Catalog = Data.ShortCircuitCatalog;
        //_manager2.Catalog = Data.FullExecutingCatalog;
    }

    //[Benchmark]
    //public void SetCatalog() => _compiler.Compile<Data.TestModel>(Data.ShortCircuitCatalog);

    //[Benchmark]
    //public void Apply1() => _compiledCatalog1.Apply(_item);

    //[Benchmark]
    //public void Apply2() => _compiledCatalog2.Apply(_item);

    [Benchmark]
    public void DetailedApply1() => _compiledCatalog1.DetailedApply(_item);

    [Benchmark]
    public void DetailedApply2() => _compiledCatalog2.DetailedApply(_item);

    //[Benchmark]
    //public void ItemSatisfiesRulesWithMessage_CircuitBreaking() => _compiledCatalog1.DetailedApply(_item);

    //[Benchmark]
    //public void ItemSatisfiesRulesWithMessage_No_CircuitBreaking() => _compiledCatalog2.DetailedApply(_item);
}
/// <summary>
/// Benchmarks in this class compare the execution of a
/// statically typed function versus one obtained by compiling a rule
/// </summary>
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

//using BenchmarkDotNet.Attributes;
//using BenchmarkDotNet.Configs;
//using BenchmarkDotNet.Running;
//using Current;
//using LogicEngine;
//using LogicEngine.Compilers;
//using LogicEngine.Internals;
//using LogicEngine.Models;
//using TinyFp;

//[MemoryDiagnoser]
//public class CompileBenchmarks
//{
//    private static readonly Data.TestModel _input = new Data.TestModel { StringProperty = "a" };
//    private static readonly RuleCompiler _compiler = new();
//    private static readonly Rule _rule = new("StringProperty", OperatorType.Equal, "some", "code");
//    private static readonly Func<Data.TestModel, Either<string, Unit>> _func = _ => Either<string, Unit>.Right(Unit.Default);
//    private CompiledRule<Data.TestModel> _compiledRule = _compiler.Compile<Data.TestModel>(_rule).Unwrap();

//    [Benchmark]
//    public void CreateRule() => _compiler.Compile<Data.TestModel>(_rule);
//}

