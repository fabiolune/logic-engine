using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using TinyFp.Extensions;

namespace Current;

internal static class Program
{
    internal static void Main() => 
        DefaultConfig
            .Instance
            .WithOption(ConfigOptions.DisableOptimizationsValidator, true)
            .Tee(o => BenchmarkRunner.Run<CompileBenchmarks>(o))
            .Tee(o => BenchmarkRunner.Run<ApplyBenchmarks>(o))
            .Tee(o => BenchmarkRunner.Run<DetailedApplyBenchmarks>(o));
}