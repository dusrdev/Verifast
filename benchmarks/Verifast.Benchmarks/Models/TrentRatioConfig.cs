using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;

namespace Verifast.Benchmarks.Models;

public class TrentRatioConfig : ManualConfig {
    public TrentRatioConfig() {
        WithOptions(ConfigOptions.DisableOptimizationsValidator);
        SummaryStyle = BenchmarkDotNet.Reports.SummaryStyle.Default.WithRatioStyle(RatioStyle.Trend);
    }
}