using BenchmarkDotNet.Attributes;

using Verifast.Benchmarks.FluentValidators;
using Verifast.Benchmarks.Models;
using Verifast.Benchmarks.VerifastValidators;

namespace Verifast.Benchmarks.Benchmarks;

[MemoryDiagnoser]
public class SyncValidationBenchmarks {
    [Params(false, true)]
    public bool UseInvalid { get; set; }

    private UserProfile _valid = null!;
    private UserProfile _invalid = null!;
    private UserProfileValidator _vfValidator = null!;
    private UserProfileFluentValidator _fvValidator = null!;

    [GlobalSetup]
    public void Setup() {
        _vfValidator = new UserProfileValidator();
        _fvValidator = new UserProfileFluentValidator();
        _valid = UserProfileFactory.CreateValid();
        _invalid = UserProfileFactory.CreateInvalid();
    }

    [Benchmark]
    public bool Verifast_Sync_TryValidate() {
        var dto = UseInvalid ? _invalid : _valid;
        return _vfValidator.TryValidate(dto, out _);
    }

    [Benchmark]
    public bool FluentValidation_Sync_Validate() {
        var dto = UseInvalid ? _invalid : _valid;
        var result = _fvValidator.Validate(dto);
        return result.IsValid;
    }
}

