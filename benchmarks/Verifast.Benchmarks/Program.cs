using BenchmarkDotNet.Running;

using Verifast.Benchmarks.Models;

namespace Verifast.Benchmarks;

public static class Program {
    public static void Main(string[] args) {
        var customConfig = new BenchmarkConfig();
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, customConfig);
    }
}