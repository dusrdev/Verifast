using System.Text.RegularExpressions;

using FluentValidation;

namespace Verifast.Benchmarks.User;

public sealed class UserProfileFluentValidator : AbstractValidator<UserProfile> {
    public UserProfileFluentValidator() {
        RuleFor(x => x.Id).NotEqual(Guid.Empty);
        RuleFor(x => x.FirstName).Must(s => !string.IsNullOrWhiteSpace(s)).MaximumLength(100);
        RuleFor(x => x.LastName).Must(s => !string.IsNullOrWhiteSpace(s)).MaximumLength(100);
        RuleFor(x => x.Email).Must(CommonValidation.IsValidEmail).WithMessage("Email is invalid.");
        RuleFor(x => x.Age).InclusiveBetween(13, 120);
        RuleFor(x => x.Address).NotNull();
        When(x => x.Address is not null, () => {
            RuleFor(x => x.Address!.Street)
                .Must(s => !string.IsNullOrWhiteSpace(s))
                .WithMessage("Street is required.");
            RuleFor(x => x.Address!.City)
                .Must(s => !string.IsNullOrWhiteSpace(s))
                .WithMessage("City is required.");
            RuleFor(x => x.Address!.Country)
                .Must(s => !string.IsNullOrWhiteSpace(s))
                .WithMessage("Country is required.");
            RuleFor(x => x.Address!.PostalCode)
                .Must(pc => !string.IsNullOrWhiteSpace(pc) && pc.Trim().Length >= 4)
                .WithMessage("PostalCode is invalid.");
        });

        RuleFor(x => x.PhoneNumbers)
            .Must(nums => nums is null || nums.Take(3).All(CommonValidation.IsValidPhone))
            .WithMessage("Phone number is invalid.");

        RuleFor(x => x.Preferences).NotNull();
        When(x => x.Preferences is not null, () => {
            RuleFor(x => x.Preferences!.Timezone)
                .Must(s => !string.IsNullOrWhiteSpace(s))
                .WithMessage("Timezone is required.");
            RuleFor(x => x.Preferences!.PreferredLanguage)
                .Must(CommonValidation.IsAllowedLanguage)
                .WithMessage("PreferredLanguage not supported.");
        });

        RuleFor(x => x.RegisteredAt).NotEqual(default(DateTimeOffset));
        RuleFor(x => x)
            .Must(x => x.LastLoginAt is null || x.LastLoginAt.Value >= x.RegisteredAt)
            .WithMessage("LastLoginAt must be >= RegisteredAt.");

        // Spot-check first order
        RuleFor(x => x.Orders).Custom((orders, ctx) => {
            if (orders is null) return;
            if (orders.Count == 0) return;

            var o = orders[0];
            if (o.OrderId == Guid.Empty) ctx.AddFailure("OrderId must not be empty.");
            if (o.Items is null || o.Items.Count == 0) {
                ctx.AddFailure("Order must have at least one item.");
                return;
            }
            var it = o.Items[0];
            if (string.IsNullOrWhiteSpace(it.Name)) ctx.AddFailure("Order item name is required.");
            if (it.Quantity <= 0) ctx.AddFailure("Order item quantity must be positive.");
            if (it.Price < 0m) ctx.AddFailure("Order item price must be non-negative.");
        });
    }
}

public readonly ref struct UserProfileVerifastValidator : IValidator<UserProfile> {
    public void Validate(in UserProfile instance, ref ValidationResult<string> result) {
        if (instance.Id == Guid.Empty)
            result.AddError("Id must not be empty.");

        if (string.IsNullOrWhiteSpace(instance.FirstName)) {
            result.AddError("FirstName is required.");
        } else if (instance.FirstName.Length > 100) {
            result.AddError("FirstName too long.");
        }

        if (string.IsNullOrWhiteSpace(instance.LastName)) {
            result.AddError("LastName is required.");
        } else if (instance.LastName.Length > 100) {
            result.AddError("LastName too long.");
        }

        if (!CommonValidation.IsValidEmail(instance.Email))
            result.AddError("Email is invalid.");

        if ((uint)instance.Age is < 13 or > 120)
            result.AddError("Age must be between 13 and 120.");

        var addr = instance.Address;
        if (addr is null) {
            result.AddError("Address is required.");
        } else {
            if (string.IsNullOrWhiteSpace(addr.Street))
                result.AddError("Street is required.");
            if (string.IsNullOrWhiteSpace(addr.City))
                result.AddError("City is required.");
            if (string.IsNullOrWhiteSpace(addr.Country))
                result.AddError("Country is required.");
            if (addr.PostalCode is null || addr.PostalCode.Trim().Length < 4)
                result.AddError("PostalCode is invalid.");
        }

        var phones = instance.PhoneNumbers;
        if (phones is not null) {
            // Validate up to first 3 numbers for demo purposes
            var count = Math.Min(phones.Count, 3);
            for (int i = 0; i < count; i++) {
                if (!CommonValidation.IsValidPhone(phones[i])) {
                    result.AddError("Phone number is invalid.");
                    break; // don't spam errors
                }
            }
        }

        if (instance.Preferences is { } pref) {
            if (string.IsNullOrWhiteSpace(pref.Timezone))
                result.AddError("Timezone is required.");
            if (!CommonValidation.IsAllowedLanguage(pref.PreferredLanguage))
                result.AddError("PreferredLanguage not supported.");
        } else {
            result.AddError("Preferences are required.");
        }

        if (instance.RegisteredAt == default)
            result.AddError("RegisteredAt is required.");
        if (instance.LastLoginAt is { } last && last < instance.RegisteredAt)
            result.AddError("LastLoginAt must be >= RegisteredAt.");

        var orders = instance.Orders;
        if (orders is not null) {
            // spot-check first order for structure
            if (orders.Count > 0) {
                var o = orders[0];
                if (o.OrderId == Guid.Empty)
                    result.AddError("OrderId must not be empty.");
                if (o.Items is null || o.Items.Count == 0) {
                    result.AddError("Order must have at least one item.");
                } else {
                    var it = o.Items[0];
                    if (string.IsNullOrWhiteSpace(it.Name)) {
                        result.AddError("Order item name is required.");
                    }
                    if (it.Quantity <= 0) {
                        result.AddError("Order item quantity must be positive.");
                    }
                    if (it.Price < 0m) {
                        result.AddError("Order item price must be non-negative.");
                    }
                }
            }
        }
    }
}

internal static partial class CommonValidation {
    [GeneratedRegex(@"^(?=.{1,254}$)(?=.{1,64}@)([A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*)@((?:[A-Za-z0-9](?:[A-Za-z0-9-]{0,61}[A-Za-z0-9])?\.)+[A-Za-z]{2,})$", RegexOptions.IgnoreCase)]
    private static partial Regex EmailRegex();

    public static bool IsValidEmail(string email) => !string.IsNullOrWhiteSpace(email) && EmailRegex().IsMatch(email);

    public static bool IsValidPhone(string phone) {
        if (string.IsNullOrWhiteSpace(phone)) return false;
        int digits = 0;
        foreach (var ch in phone) {
            if (ch is >= '0' and <= '9') digits++;
        }
        return digits >= 10;
    }

    public static bool IsAllowedLanguage(string lang) => lang is "en" or "es" or "fr" or "de";
}