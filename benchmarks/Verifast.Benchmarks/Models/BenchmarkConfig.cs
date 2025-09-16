using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;

namespace Verifast.Benchmarks.Models;

public class BenchmarkConfig : ManualConfig {
    public BenchmarkConfig() {
        SummaryStyle = SummaryStyle.Default.WithRatioStyle(RatioStyle.Trend);
        AddDiagnoser(MemoryDiagnoser.Default);
        AddJob(Job.MediumRun);
        AddColumn(RankColumn.Arabic);
        HideColumns(Column.Error, Column.StdDev, Column.Median, Column.RatioSD);
    }
}
