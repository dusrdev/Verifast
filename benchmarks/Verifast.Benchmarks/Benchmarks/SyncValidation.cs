using BenchmarkDotNet.Attributes;

using Verifast.Benchmarks.User;

namespace Verifast.Benchmarks.Benchmarks;

[ReturnValueValidator]
public class SyncValidation {
    [Params(true, false)]
    public bool ValidDto { get; set; }

    private UserProfile? _dto;

    [GlobalSetup]
    public void Setup() {
        _dto = ValidDto
            ? UserProfileFactory.CreateValid()
            : UserProfileFactory.CreateInvalid();
    }

    [Benchmark(Baseline = true, Description = "FluentValidation")]
    public int FluentValidation() {
        var validator = new UserProfileFluentValidator();
        var result = validator.Validate(_dto!);
        return result.Errors.Count;
    }

    [Benchmark(Description = "Verifast")]
    public int Verifast() {
        var validator = new UserProfileVerifastValidator();
        var result = validator.Validate(_dto!);
        return result.Errors.Count;
    }
}