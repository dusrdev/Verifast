using BenchmarkDotNet.Attributes;

using Verifast.Benchmarks.User;

namespace Verifast.Benchmarks.Benchmarks;

public class AsyncValidation {
    [Params(true, false)]
    public bool ValidDto { get; set; }

    private UserProfile? _dto;
    private FakeUserRepository _repo = null!;

    [GlobalSetup]
    public async Task Setup() {
        // Seed repo with a taken email and default blacklist
        _repo = new FakeUserRepository(seedEmails: ["taken@spam.com"]);
        // Simulate seeding work
        await _repo.AddAsync("taken@spam.com");

        _dto = ValidDto
            ? UserProfileFactory.CreateValid("valid@example.com")
            : UserProfileFactory.CreateInvalid("taken@spam.com");
    }

    [Benchmark(Baseline = true, Description = "FluentValidation")]
    public async Task<int> FluentValidation_Async() {
        var validator = new UserProfileFluentAsyncValidator(_repo);
        var result = await validator.ValidateAsync(_dto!);
        return result.Errors.Count;
    }

    [Benchmark(Description = "Verifast")]
    public async Task<int> Verifast_Async() {
        var validator = new AsyncUserProfileVerifastValidator(_repo);
        var result = await validator.ValidateAsync(_dto!);
        return result.Errors.Count;
    }
}