namespace Verifast;

// Single-parameter interface. Implementers only specify the model type.
public interface IValidator<T> where T : allows ref struct {
    // Implementers fill the provided result. No allocations unless needed.
    void Validate(in T instance, ref ValidationResult result);
}

public interface IAsyncValidator<T> {
    ValueTask<ValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken = default);
}

public static class Validator {
    public static ValidationResult Validate<TValidator, T>(this TValidator validator, in T instance)
        where TValidator : IValidator<T>, allows ref struct
        where T : allows ref struct {
        ValidationResult result = default;
        validator.Validate(in instance, ref result);
        return result;
    }

    public static bool TryValidate<TValidator, T>(this TValidator validator, in T instance, out ValidationResult result)
        where TValidator : IValidator<T>, allows ref struct
        where T : allows ref struct {
        result = default;
        validator.Validate(in instance, ref result);
        return result.IsValid;
    }
}