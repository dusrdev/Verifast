using System.Collections.ObjectModel;

namespace Verifast;

/// <summary>
/// Validation result
/// </summary>
public struct ValidationResult<TMessage> {
    private List<TMessage>? _errors;
    private List<TMessage>? _warnings;

    /// <summary>
    /// The result is valid if no errors were found.
    /// </summary>
    public readonly bool IsValid => _errors is null or { Count: 0 };

    /// <summary>
    /// A <see cref="ReadOnlyCollection{TMessage}"/> of the <see cref="Errors"/>
    /// </summary>
    public readonly ReadOnlyCollection<TMessage> Errors
        => _errors is not null
        ? new ReadOnlyCollection<TMessage>(_errors)
        : ReadOnlyCollection<TMessage>.Empty;

    /// <summary>
    /// A <see cref="ReadOnlyCollection{TMessage}"/> of the <see cref="Warnings"/>
    /// </summary>
    public readonly ReadOnlyCollection<TMessage> Warnings
        => _warnings is not null
        ? new ReadOnlyCollection<TMessage>(_warnings)
        : ReadOnlyCollection<TMessage>.Empty;

    /// <summary>
    /// Adds an error to the validation result
    /// </summary>
    /// <param name="message"></param>
    public void AddError(TMessage message) {
        _errors ??= new List<TMessage>();
        _errors.Add(message);
    }

    /// <summary>
    /// Adds a warning to the validation result
    /// </summary>
    /// <param name="message"></param>
    public void AddWarning(TMessage message) {
        _warnings ??= new List<TMessage>();
        _warnings.Add(message);
    }
}