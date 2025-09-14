namespace Verifast;

/// <summary>
/// Interface for implementing a synchronous validator for <typeparamref name="T"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IValidator<T> where T : allows ref struct {
    /// <summary>
	/// Validate method
	/// </summary>
	/// <param name="instance"></param>
	/// <param name="result"></param>
    void Validate(in T instance, ref ValidationResult result);
}

/// <summary>
/// Interface for implementing an asynchronous validator for <typeparamref name="T"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IAsyncValidator<T> {
    /// <summary>
    /// ValidateAsync method
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask<ValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken = default);
}

/// <summary>
/// A static class providing the main APIs for validation
/// </summary>
public static class Validator {
    /// <summary>
    /// Validate
    /// </summary>
    /// <typeparam name="TValidator"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="validator"></param>
    /// <param name="instance"></param>
    /// <returns></returns>
    public static ValidationResult Validate<TValidator, T>(this TValidator validator, in T instance)
        where TValidator : IValidator<T>, allows ref struct
        where T : allows ref struct {
        ValidationResult result = default;
        validator.Validate(in instance, ref result);
        return result;
    }

    /// <summary>
    /// Try Validate
    /// </summary>
    /// <typeparam name="TValidator"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="validator"></param>
    /// <param name="instance"></param>
    /// <param name="result"></param>
    /// <returns>True if <paramref name="instance"/> is valid.</returns>
    public static bool TryValidate<TValidator, T>(this TValidator validator, in T instance, out ValidationResult result)
        where TValidator : IValidator<T>, allows ref struct
        where T : allows ref struct {
        result = default;
        validator.Validate(in instance, ref result);
        return result.IsValid;
    }
}