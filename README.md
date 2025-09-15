# Verifast

Fast, allocation‑friendly validation for .NET 9 with a focused, interface‑driven API. Verifast centers on simple validators and a lightweight result type that captures errors and warnings on demand.

## Features

- Lean contracts: `IValidator<T, TMessage>`, `IValidator<T>`, `IAsyncValidator<T, TMessage>`, `IAsyncValidator<T>`.
- Lightweight results: struct based `ValidationResult<TMessage>` tracks validity, errors, and warnings with minimal allocations.
- Orchestrator helpers: static `Validator` extensions to run validation and `TryValidate` patterns.
- Ref‑friendly design: contracts are compatible with `ref struct` scenarios.

## Getting Started

### Download via nuget

```bash
dotnet add package Verifast
```

## Usage

Below is a minimal example showing the synchronous contract and the orchestrator helpers.

```csharp
using Verifast;

public readonly record struct Person(string Name, int Age);

public readonly struct PersonValidator : IValidator<Person>
{
    public ValidationResult<string> Validate(in Person person)
    {
        var result = ValidationResult.Create<string>();

        if (string.IsNullOrWhiteSpace(person.Name))
        {
            result.AddError("Name is required.");
        }

        if (person.Age < 0 || person.Age > 130)
        {
            result.AddError("Age must be between 0 and 130.");
        }

        return result;
    }
}

// Run validation
var person = new Person("Ada", 33);
var validator = new PersonValidator();

// Orchestrator helpers
if (!validator.TryValidate(person, out var result))
{
    foreach (var error in result.Errors)
    {
        Console.WriteLine(error);
    }
}
```

### Async validators
```csharp
public readonly struct PersonAsyncValidator : IAsyncValidator<Person>
{
    public async ValueTask<ValidationResult<string>> ValidateAsync(Person person, CancellationToken ct = default)
    {
        // Perform async checks as needed
        await Task.Yield();

        var result = ValidationResult.Create<string>();
        if (string.IsNullOrWhiteSpace(person.Name))
        {
            result.AddError("Name is required.");
        }
        return result;
    }
}
```

### Custom message types
Use `TMessage` to avoid string allocations or to carry rich metadata.

```csharp
public readonly record struct ValidationMessage(string Code, string Text);

public readonly struct CustomValidator : IValidator<int, ValidationMessage>
{
    public ValidationResult<ValidationMessage> Validate(in int value)
    {
        var result = ValidationResult.Create<ValidationMessage>();
        if (value % 2 != 0)
        {
            result.AddError(new ValidationMessage("NotEven", "Value must be even."));
        }
        return result;
    }
}
```

## Running Tests
Per repository conventions, run tests via `dotnet run` (builds implicitly):

```
dotnet run --project tests/Verifast.Tests.Unit
```

List tests (Microsoft Testing Platform semantics):
```
dotnet run --project tests/Verifast.Tests.Unit -- --list-tests
```

Run a single test by method or class filter:
```
dotnet run --project tests/Verifast.Tests.Unit --filter-method="*TestMethodPattern*"
dotnet run --project tests/Verifast.Tests.Unit --filter-class="*TestClassPattern*"
```

## Formatting
Analyze only:
```
dotnet format analyze --severity info
```

Apply style/whitespace fixes:
```
dotnet format
```

If `dotnet format` is missing, install the tool:
```
dotnet tool update -g dotnet-format
```

## Design Notes
- Contracts use constraints that allow validators to work with `ref struct` scenarios without forcing `ref struct` everywhere.
- `ValidationResult<TMessage>` captures errors/warnings on demand to minimize allocations.
- Numeric helpers use generic math (`INumber<T>`) where appropriate.

## License
MIT — see `LICENSE` for details.

