using System.Collections.ObjectModel;

namespace Verifast;

/// <summary>
/// Validation result
/// </summary>
public struct ValidationResult {
    private List<string>? _errors;
    private List<string>? _warnings;

    /// <summary>
    /// The result is valid if no errors were found.
    /// </summary>
    public readonly bool IsValid => _errors is null or { Count: 0 };

    /// <summary>
    /// A <see cref="ReadOnlyCollection{string}"/> of the <see cref="Errors"/>
    /// </summary>
    public readonly ReadOnlyCollection<string> Errors
        => _errors is not null
        ? new ReadOnlyCollection<string>(_errors)
        : ReadOnlyCollection<string>.Empty;

    /// <summary>
    /// A <see cref="ReadOnlyCollection{string}"/> of the <see cref="Warnings"/>
    /// </summary>
    public readonly ReadOnlyCollection<string> Warnings
        => _warnings is not null
        ? new ReadOnlyCollection<string>(_warnings)
        : ReadOnlyCollection<string>.Empty;

    /// <summary>
    /// Adds an error to the validation result
    /// </summary>
    /// <param name="message"></param>
    public void AddError(string message) {
        _errors ??= new List<string>();
        _errors.Add(message);
    }

    /// <summary>
    /// Adds a warning to the validation result
    /// </summary>
    /// <param name="message"></param>
    public void AddWarning(string message) {
        _warnings ??= new List<string>();
        _warnings.Add(message);
    }
}