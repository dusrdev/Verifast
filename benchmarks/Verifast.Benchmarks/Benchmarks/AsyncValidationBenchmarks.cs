using BenchmarkDotNet.Attributes;
using Verifast.Benchmarks.FluentValidators;
using Verifast.Benchmarks.Infrastructure;
using Verifast.Benchmarks.Models;
using Verifast.Benchmarks.VerifastValidators;

namespace Verifast.Benchmarks.Benchmarks;

[MediumRunJob]
[MemoryDiagnoser]
[RankColumn]
[ReturnValueValidator]
[Config(typeof(TrentRatioConfig))]
public class AsyncValidationBenchmarks {
    [Params(true, false)]
    public bool DtoValid { get; set; }

    private UserProfile? _dto;
    private FakeUserRepository _repo = null!;

    [GlobalSetup]
    public async Task Setup() {
        // Seed repo with a taken email and default blacklist
        _repo = new FakeUserRepository(seedEmails: ["taken@spam.com"]);
        // Simulate seeding work
        await _repo.AddAsync("taken@spam.com");

        _dto = DtoValid
            ? UserProfileFactory.CreateValid("valid@example.com")
            : UserProfileFactory.CreateInvalid("taken@spam.com");
    }

    [Benchmark(Baseline = true)]
    public async Task<int> FluentValidation_Async() {
        var validator = new UserProfileFluentAsyncValidator(_repo);
        var result = await validator.ValidateAsync(_dto!);
        return result.Errors.Count;
    }

    [Benchmark]
    public async Task<int> Verifast_Async() {
        var validator = new AsyncUserProfileVerifastValidator(_repo);
        var result = await validator.ValidateAsync(_dto!);
        return result.Errors.Count;
    }
}

