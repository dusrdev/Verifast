namespace Verifast;

/// <summary>
/// Interface for implementing an asynchronous validator for <typeparamref name="T"/> with message type <typeparamref name="TMessage"/>
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TMessage"></typeparam>
public interface IAsyncValidator<in T, TMessage> {
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
public interface IAsyncValidator<in T> {
    /// <summary>
    /// ValidateAsync method
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    ValueTask<ValidationResult<string>> ValidateAsync(T instance, CancellationToken ct = default);
}