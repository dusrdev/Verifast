namespace Verifast;

/// <summary>
/// Interface for implementing a synchronous validator for <typeparamref name="T"/> with message type <typeparamref name="TMessage"/>
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TMessage"></typeparam>
public interface IValidator<T, TMessage> where T : allows ref struct {
    /// <summary>
    /// Validate method
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="result"></param>
    void Validate(in T instance, ref ValidationResult<TMessage> result);
}

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
    void Validate(in T instance, ref ValidationResult<string> result);
}

/// <summary>
/// Interface for implementing an asynchronous validator for <typeparamref name="T"/> with message type <typeparamref name="TMessage"/>
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TMessage"></typeparam>
public interface IAsyncValidator<T, TMessage> {
    /// <summary>
    /// ValidateAsync method
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    ValueTask<ValidationResult<TMessage>> ValidateAsync(T instance, CancellationToken ct = default);
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
    /// <param name="ct"></param>
    /// <returns></returns>
    ValueTask<ValidationResult<string>> ValidateAsync(T instance, CancellationToken ct = default);
}