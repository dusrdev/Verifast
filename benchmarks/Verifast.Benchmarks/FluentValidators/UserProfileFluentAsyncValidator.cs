using FluentValidation;
using Verifast.Benchmarks.Infrastructure;
using Verifast.Benchmarks.Models;

namespace Verifast.Benchmarks.FluentValidators;

public sealed class UserProfileFluentAsyncValidator : AbstractValidator<UserProfile> {
    public UserProfileFluentAsyncValidator(FakeUserRepository repo) {
        RuleFor(x => x.Email)
            .NotEmpty()
            .MustAsync(repo.IsDomainAllowedAsync)
            .WithMessage("Email domain is not allowed.")
            .MustAsync(repo.IsEmailUniqueAsync)
            .WithMessage("Email is already taken.");

        RuleFor(x => x.Age).GreaterThanOrEqualTo(13);
        RuleFor(x => x.RegisteredAt).NotEqual(default(DateTimeOffset));
    }
}

