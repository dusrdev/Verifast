namespace Verifast.Tests.Unit;

public class AsyncUserDto : IAsyncValidator<AsyncUserDto> {
    public string? Name { get; set; }
    public int Age { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Phone { get; set; }

    public async ValueTask<ValidationResult> ValidateAsync(AsyncUserDto instance, CancellationToken cancellationToken = default) {
        // Emulate asynchronous work so tests exercise the async path
        await Task.Yield();
        cancellationToken.ThrowIfCancellationRequested();

        var result = new ValidationResult();

        // Name: required, length 1..100
        if (string.IsNullOrWhiteSpace(instance.Name)) {
            result.AddError($"'{nameof(Name)}' must be non-empty");
        } else if (instance.Name!.Length > 100) {
            result.AddError($"'{nameof(Name)}' must be at most 100 characters");
        }

        // Age: 18..120 inclusive
        if (instance.Age is < 18 or > 120)
            result.AddError($"'{nameof(Age)}' must be between 18 and 120");

        // Email: very lightweight format check (no regex)
        if (!string.IsNullOrWhiteSpace(instance.Email)) {
            if (!IsLikelyEmail(instance.Email!))
                result.AddError($"'{nameof(Email)}' must be a valid email");
        }

        // Password: required, min 8, has upper, lower, digit
        if (string.IsNullOrEmpty(instance.Password)) {
            result.AddError($"'{nameof(Password)}' must be non-empty");
        } else {
            var pwd = instance.Password!;
            if (pwd.Length < 8)
                result.AddError($"'{nameof(Password)}' must be at least 8 characters");

            bool hasUpper = false, hasLower = false, hasDigit = false;
            for (int i = 0; i < pwd.Length; i++) {
                char c = pwd[i];
                if (!hasUpper && char.IsUpper(c)) hasUpper = true;
                else if (!hasLower && char.IsLower(c)) hasLower = true;
                else if (!hasDigit && char.IsDigit(c)) hasDigit = true;
                if (hasUpper && hasLower && hasDigit) break;
            }
            if (!hasUpper) result.AddError($"'{nameof(Password)}' must contain an uppercase letter");
            if (!hasLower) result.AddError($"'{nameof(Password)}' must contain a lowercase letter");
            if (!hasDigit) result.AddError($"'{nameof(Password)}' must contain a digit");
        }

        // Phone: optional; if provided, basic E.164-ish (+?[0-9]{10,15})
        if (!string.IsNullOrWhiteSpace(instance.Phone)) {
            if (!IsLikelyPhone(instance.Phone!))
                result.AddWarning($"'{nameof(Phone)}' format looks invalid");
        }

        return result;

        static bool IsLikelyEmail(string s) {
            // Minimal: exactly one '@', non-empty local/domain, domain has a '.' after '@'
            int at = s.IndexOf('@');
            if (at <= 0 || at >= s.Length - 1) return false;
            int dot = s.IndexOf('.', at + 1);
            if (dot <= at + 1 || dot >= s.Length - 1) return false;
            return true;
        }

        static bool IsLikelyPhone(string s) {
            int i = 0; int digits = 0;
            if (s.Length > 0 && s[0] == '+') i = 1;
            for (; i < s.Length; i++) {
                char c = s[i];
                if (char.IsDigit(c)) { digits++; continue; }
                return false;
            }
            return digits >= 10 && digits <= 15;
        }
    }
}