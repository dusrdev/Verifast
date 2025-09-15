using BenchmarkDotNet.Attributes;
using Verifast.Benchmarks.FluentValidators;
using Verifast.Benchmarks.Infrastructure;
using Verifast.Benchmarks.Models;
using Verifast.Benchmarks.VerifastValidators;

namespace Verifast.Benchmarks.Benchmarks;

[MemoryDiagnoser]
public class AsyncValidationBenchmarks {
    [Params(false, true)]
    public bool UseInvalid { get; set; }

    private UserProfile _valid = null!;
    private UserProfile _invalid = null!;
    private AsyncUserProfileValidator _vfValidator = null!;
    private UserProfileFluentAsyncValidator _fvValidator = null!;
    private FakeUserRepository _repo = null!;

    [GlobalSetup]
    public async Task Setup() {
        // Seed repo with a taken email and default blacklist
        _repo = new FakeUserRepository(seedEmails: ["taken@spam.com"]);
        // Simulate seeding work
        await _repo.AddAsync("taken@spam.com");

        _vfValidator = new AsyncUserProfileValidator(_repo);
        _fvValidator = new UserProfileFluentAsyncValidator(_repo);

        _valid = UserProfileFactory.CreateValid("valid@example.com");
        _invalid = UserProfileFactory.CreateInvalid("taken@spam.com");
    }

    [Benchmark]
    public async Task<bool> Verifast_Async_Validate() {
        var dto = UseInvalid ? _invalid : _valid;
        var result = await _vfValidator.ValidateAsync(dto);
        return result.IsValid;
    }

    [Benchmark]
    public async Task<bool> FluentValidation_Async_Validate() {
        var dto = UseInvalid ? _invalid : _valid;
        var result = await _fvValidator.ValidateAsync(dto);
        return result.IsValid;
    }
}

