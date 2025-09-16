using BenchmarkDotNet.Attributes;

using Verifast.Benchmarks.User;
using Verifast.Benchmarks.Models;

namespace Verifast.Benchmarks.Benchmarks;

[ReturnValueValidator]
[Config(typeof(BenchmarkConfig))]
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
        var validator = new UserProfileVerifastValidator();
        var result = validator.Validate(_dto!);
        return result.Errors.Count;
    }
}

