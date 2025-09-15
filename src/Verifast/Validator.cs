namespace Verifast;

/// <summary>
/// A static class providing the main APIs for validation
/// </summary>
public static class Validator {
    /// <summary>
    /// Validate
    /// </summary>
    /// <typeparam name="TValidator"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="validator"></param>
    /// <param name="instance"></param>
    /// <returns></returns>
    public static ValidationResult<TMessage> Validate<TValidator, T, TMessage>(this TValidator validator, in T instance)
        where TValidator : IValidator<T, TMessage>, allows ref struct
        where T : allows ref struct {
        ValidationResult<TMessage> result = default;
        validator.Validate(in instance, ref result);
        return result;
    }

    /// <summary>
	/// Validate
	/// </summary>
	/// <typeparam name="TValidator"></typeparam>
	/// <typeparam name="T"></typeparam>
	/// <param name="validator"></param>
	/// <param name="instance"></param>
	/// <returns></returns>
	public static ValidationResult<string> Validate<TValidator, T>(this TValidator validator, in T instance)
        where TValidator : IValidator<T>, allows ref struct
        where T : allows ref struct {
        ValidationResult<string> result = default;
        validator.Validate(in instance, ref result);
        return result;
    }

    /// <summary>
    /// Try Validate
    /// </summary>
    /// <typeparam name="TValidator"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="validator"></param>
    /// <param name="instance"></param>
    /// <param name="result"></param>
    /// <returns>True if <paramref name="instance"/> is valid.</returns>
    public static bool TryValidate<TValidator, T, TMessage>(this TValidator validator, in T instance, out ValidationResult<TMessage> result)
        where TValidator : IValidator<T, TMessage>, allows ref struct
        where T : allows ref struct {
        result = default;
        validator.Validate(in instance, ref result);
        return result.IsValid;
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
	public static bool TryValidate<TValidator, T>(this TValidator validator, in T instance, out ValidationResult<string> result)
        where TValidator : IValidator<T>, allows ref struct
        where T : allows ref struct {
        result = default;
        validator.Validate(in instance, ref result);
        return result.IsValid;
    }
}