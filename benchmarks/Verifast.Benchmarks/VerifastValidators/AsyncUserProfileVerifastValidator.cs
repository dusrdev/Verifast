using Verifast.Benchmarks.Infrastructure;
using Verifast.Benchmarks.Models;

namespace Verifast.Benchmarks.VerifastValidators;

public sealed class AsyncUserProfileVerifastValidator : IAsyncValidator<UserProfile> {
    private readonly FakeUserRepository _repo;

    public AsyncUserProfileVerifastValidator(FakeUserRepository repo) {
        _repo = repo;
    }

    public async ValueTask<ValidationResult<string>> ValidateAsync(UserProfile instance, CancellationToken ct = default) {
        ValidationResult<string> result = default;

        if (string.IsNullOrWhiteSpace(instance.Email)) {
            result.AddError("Email is required.");
            return result;
        }

        // Simulate async database validations
        var domainAllowed = await _repo.IsDomainAllowedAsync(instance.Email, ct);
        if (!domainAllowed) {
            result.AddError("Email domain is not allowed.");
        }

        var unique = await _repo.IsEmailUniqueAsync(instance.Email, ct);
        if (!unique) {
            result.AddError("Email is already taken.");
        }

        // A couple of synchronous quick checks inline for realism
        if (instance.RegisteredAt == default) {
            result.AddError("RegisteredAt is required.");
        }
        if (instance.Age < 13) {
            result.AddError("Age must be at least 13.");
        }

        return result;
    }
}