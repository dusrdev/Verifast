using FluentValidation;
using Verifast.Benchmarks.Models;

namespace Verifast.Benchmarks.FluentValidators;

public sealed class UserProfileFluentValidator : AbstractValidator<UserProfile> {
    public UserProfileFluentValidator() {
        RuleFor(x => x.Id).NotEqual(Guid.Empty);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Age).InclusiveBetween(13, 120);
        RuleFor(x => x.Address).NotNull();
        When(x => x.Address is not null, () => {
            RuleFor(x => x.Address!.Street).NotEmpty();
            RuleFor(x => x.Address!.City).NotEmpty();
            RuleFor(x => x.Address!.Country).NotEmpty();
            RuleFor(x => x.Address!.PostalCode).NotEmpty().MinimumLength(4);
        });

        RuleFor(x => x.PhoneNumbers)
            .Must(nums => nums is null || nums.Take(3).All(IsValidPhone))
            .WithMessage("Phone number is invalid.");

        RuleFor(x => x.Preferences).NotNull();
        When(x => x.Preferences is not null, () => {
            RuleFor(x => x.Preferences!.Timezone).NotEmpty();
            RuleFor(x => x.Preferences!.PreferredLanguage)
                .Must(lang => lang is "en" or "es" or "fr" or "de")
                .WithMessage("PreferredLanguage not supported.");
        });

        RuleFor(x => x.RegisteredAt).NotEqual(default(DateTimeOffset));
        RuleFor(x => x)
            .Must(x => x.LastLoginAt is null || x.LastLoginAt.Value >= x.RegisteredAt)
            .WithMessage("LastLoginAt must be >= RegisteredAt.");

        // Spot-check first order
        RuleFor(x => x.Orders)
            .Must(orders => orders is null || orders.Count == 0 || ValidateFirstOrder(orders[0]))
            .WithMessage("Order validation failed.");
    }

    private static bool IsValidPhone(string phone) {
        if (string.IsNullOrWhiteSpace(phone)) {
            return false;
        }
        int digits = 0;
        foreach (var ch in phone) {
            if (ch is >= '0' and <= '9') {
                digits++;
            }
        }
        return digits >= 10;
    }

    private static bool ValidateFirstOrder(Order o) {
        if (o.OrderId == Guid.Empty) {
            return false;
        }
        if (o.Items is null || o.Items.Count == 0) {
            return false;
        }
        var it = o.Items[0];
        if (string.IsNullOrWhiteSpace(it.Name) || it.Quantity <= 0 || it.Price < 0m) {
            return false;
        }
        decimal sum = 0m;
        for (int i = 0; i < o.Items.Count; i++) {
            var item = o.Items[i];
            sum += item.Price * item.Quantity;
        }
        if (o.Total != 0m && Math.Abs(o.Total - sum) > 0.001m) {
            return false;
        }
        return true;
    }
}

