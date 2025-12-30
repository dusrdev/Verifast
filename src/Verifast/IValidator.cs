namespace Verifast;

#if NET9_0_OR_GREATER
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
#else
/// <summary>
/// Interface for implementing a synchronous validator for <typeparamref name="T"/> with message type <typeparamref name="TMessage"/>
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TMessage"></typeparam>
public interface IValidator<T, TMessage> {
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
public interface IValidator<T> {
    /// <summary>
	/// Validate method
	/// </summary>
	/// <param name="instance"></param>
	/// <param name="result"></param>
    void Validate(in T instance, ref ValidationResult<string> result);
}
#endif