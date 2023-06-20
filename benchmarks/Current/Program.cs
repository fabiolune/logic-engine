using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace Current;

internal static class Program
{
    internal static void Main()
    {
        //BenchmarkRunner
        //    .Run<CompileBenchmarks>(DefaultConfig.Instance.WithOption(ConfigOptions.DisableOptimizationsValidator, true));

        BenchmarkRunner
            .Run<ApplyBenchmarks>(DefaultConfig.Instance.WithOption(ConfigOptions.DisableOptimizationsValidator, true));

        //BenchmarkRunner
        //    .Run<DetailedApplyBenchmarks>(DefaultConfig.Instance.WithOption(ConfigOptions.DisableOptimizationsValidator, true));
    }
}