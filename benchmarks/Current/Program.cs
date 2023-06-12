using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace Current;

internal static class Program
{
    internal static void Main() => 
        BenchmarkRunner
            .Run<CurrentImplementationBenchmarks>(DefaultConfig.Instance.WithOption(ConfigOptions.DisableOptimizationsValidator, true));
}