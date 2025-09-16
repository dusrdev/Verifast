using FluentValidation;

namespace Verifast.Benchmarks.User;

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

public readonly ref struct UserProfileVerifastValidator : IValidator<UserProfile> {
    public void Validate(in UserProfile instance, ref ValidationResult<string> result) {
        if (instance.Id == Guid.Empty) {
            result.AddError("Id must not be empty.");
        }

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

        if (!LooksLikeEmail(instance.Email)) {
            result.AddError("Email is invalid.");
        }

        if ((uint)instance.Age is < 13 or > 120) {
            result.AddError("Age must be between 13 and 120.");
        } else if (instance.Age < 18) {
            result.AddWarning("Age is under 18.");
        }

        var addr = instance.Address;
        if (addr is null) {
            result.AddError("Address is required.");
        } else {
            if (string.IsNullOrWhiteSpace(addr.Street)) {
                result.AddError("Street is required.");
            }
            if (string.IsNullOrWhiteSpace(addr.City)) {
                result.AddError("City is required.");
            }
            if (string.IsNullOrWhiteSpace(addr.Country)) {
                result.AddError("Country is required.");
            }
            if (addr.PostalCode is null || addr.PostalCode.Trim().Length < 4) {
                result.AddError("PostalCode is invalid.");
            }
        }

        var phones = instance.PhoneNumbers;
        if (phones is not null) {
            // Validate up to first 3 numbers for demo purposes
            var count = Math.Min(phones.Count, 3);
            for (int i = 0; i < count; i++) {
                if (!LooksLikePhone(phones[i])) {
                    result.AddError("Phone number is invalid.");
                    break; // don't spam errors
                }
            }
        }

        if (instance.Preferences is { } pref) {
            if (string.IsNullOrWhiteSpace(pref.Timezone)) {
                result.AddError("Timezone is required.");
            }
            if (!IsAllowedLanguage(pref.PreferredLanguage)) {
                result.AddError("PreferredLanguage not supported.");
            }
        } else {
            result.AddError("Preferences are required.");
        }

        if (instance.RegisteredAt == default) {
            result.AddError("RegisteredAt is required.");
        }
        if (instance.LastLoginAt is { } last && last < instance.RegisteredAt) {
            result.AddError("LastLoginAt must be >= RegisteredAt.");
        }

        var orders = instance.Orders;
        if (orders is not null) {
            if (orders.Count > 100) {
                result.AddWarning("Unusually high number of orders.");
            }
            // spot-check first order for structure
            if (orders.Count > 0) {
                var o = orders[0];
                if (o.OrderId == Guid.Empty) {
                    result.AddError("OrderId must not be empty.");
                }
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
                    // Check total matches sum (first order only to keep work bounded)
                    decimal sum = 0m;
                    for (int i = 0; i < o.Items.Count; i++) {
                        var item = o.Items[i];
                        sum += item.Price * item.Quantity;
                    }
                    if (o.Total != 0m && Math.Abs(o.Total - sum) > 0.001m) {
                        result.AddWarning("Order total doesn't match sum of items.");
                    }
                }
            }
        }
    }

    private static bool LooksLikeEmail(string email) {
        if (string.IsNullOrWhiteSpace(email)) {
            return false;
        }
        var span = email.AsSpan();
        var at = span.IndexOf('@');
        if (at <= 0 || at >= span.Length - 3) {
            return false;
        }
        var dot = span[(at + 1)..].LastIndexOf('.');
        return dot > 0;
    }

    private static bool LooksLikePhone(string phone) {
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

    private static bool IsAllowedLanguage(string lang) {
        return lang is "en" or "es" or "fr" or "de";
    }
}