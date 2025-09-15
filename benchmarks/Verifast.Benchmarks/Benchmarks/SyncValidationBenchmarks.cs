using BenchmarkDotNet.Attributes;

using Verifast.Benchmarks.FluentValidators;
using Verifast.Benchmarks.Models;
using Verifast.Benchmarks.VerifastValidators;

namespace Verifast.Benchmarks.Benchmarks;

[MediumRunJob]
[MemoryDiagnoser]
[RankColumn]
[ReturnValueValidator]
[Config(typeof(TrentRatioConfig))]
public class SyncValidationBenchmarks {
    [Params(true, false)]
    public bool DtoValid { get; set; }

    private UserProfile? _dto;

    [GlobalSetup]
    public void Setup() {
        _dto = DtoValid
            ? UserProfileFactory.CreateValid()
            : UserProfileFactory.CreateInvalid();
    }

    [Benchmark(Baseline = true)]
    public int FluentValidation() {
        var validator = new UserProfileFluentValidator();
        var result = validator.Validate(_dto!);
        return result.Errors.Count;
    }

    [Benchmark]
    public int Verifast() {
        var validator = new UserProfileValidator();
        var result = validator.Validate(_dto!);
        return result.Errors.Count;
    }
}

