using System.Collections.ObjectModel;

namespace Verifast;

public struct ValidationResult {
	private List<string>? _errors;
	private List<string>? _warnings;

	public readonly bool IsValid => _errors is null or { Count: 0 };

	// Expose as read-only views to discourage mutation by callers
	public readonly ReadOnlyCollection<string> Errors
		=> _errors is not null
		? new ReadOnlyCollection<string>(_errors)
		: ReadOnlyCollection<string>.Empty;
	public readonly ReadOnlyCollection<string> Warnings
		=> _warnings is not null
		? new ReadOnlyCollection<string>(_warnings)
		: ReadOnlyCollection<string>.Empty;

	// Convenience helpers to avoid touching the lists directly
	public void AddError(string message) {
		_errors ??= new List<string>();
		_errors.Add(message);
	}
	public void AddWarning(string message) {
		_warnings ??= new List<string>();
		_warnings.Add(message);
	}
}
