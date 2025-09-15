# Verifast

High‑performance, allocation‑friendly validation for .NET >= 9. No complicated APIs, no expression trees — just plain C#. Implement a tiny interface, add errors or warnings, and you’re done.

## Why Verifast

- Minimal API: implement `IValidator<T>` or `IAsyncValidator<T>` and write ordinary C#.
- Fast by design: struct based `ValidationResult<TMessage>` allocates only when you add messages.
- Ergonomic helpers: call `.Validate(...)` for a result or `.TryValidate(...)` for a quick bool.
- Plays well with `ref struct`: APIs are designed to enable stack‑only scenarios.
- Your messages, your way: use `string` or a custom `TMessage` for richer metadata.

## Quick Start

Define validation by implementing the interface on your type (or on a separate validator). Then call the extension helpers.

```csharp
using Verifast;

public sealed class User : IValidator<User> {
    public string? Name { get; init; }
    public int Age { get; init; }

    public void Validate(in User instance, ref ValidationResult<string> result) {
        if (string.IsNullOrWhiteSpace(instance.Name))
            result.AddError("'Name' must be non-empty");

        if (instance.Age is < 18 or > 120)
            result.AddError("'Age' must be between 18 and 120");
    }
}

var user = new User { Name = "Ada", Age = 33 };
var result = user.Validate(user);
if (!result.IsValid)
    foreach (var error in result.Errors) Console.WriteLine(error);
```

Or get a simple pass/fail while still capturing details:

```csharp
if (!user.TryValidate(user, out var result))
    foreach (var error in result.Errors) Console.WriteLine(error);
```

## Async Validation

Prefer async? Implement `IAsyncValidator<T>` and return a `ValueTask<ValidationResult<TMessage>>`.

```csharp
using Verifast;

public sealed class AsyncUser : IAsyncValidator<AsyncUser> {
    public string? Email { get; init; }

    public async ValueTask<ValidationResult<string>> ValidateAsync(AsyncUser instance, CancellationToken ct = default) {
        await Task.Yield(); // e.g., call a store or service

        ValidationResult<string> result = default; // valid by default struct
        if (!string.IsNullOrWhiteSpace(instance.Email))
            if (!LooksLikeEmail(instance.Email!))
                result.AddError($"'{nameof(Email)}' must be a valid email");

        return result;

        static bool LooksLikeEmail(string s) {
            int at = s.IndexOf('@');
            if (at <= 0 || at >= s.Length - 1) return false;
            int dot = s.IndexOf('.', at + 1);
            if (dot <= at + 1 || dot >= s.Length - 1) return false;
            return true;
        }
    }
}
```

## Custom Message Types

Avoid string allocations or carry structured metadata by choosing your own `TMessage`.

```csharp
using Verifast;

public readonly record struct Msg(string Code, string Text);

public readonly ref struct EvenValidator : IValidator<int, Msg> {
    public void Validate(in int value, ref ValidationResult<Msg> result) {
        if ((value & 1) != 0)
            result.AddError(new Msg("NotEven", "Value must be even"));
    }
}

if (!new EvenValidator().TryValidate(3, out var res))
    foreach (var e in res.Errors) Console.WriteLine($"{e.Code}: {e.Text}");
```

## Design Philosophy

- Pure language constructs: write straightforward `if`/`for` statements — no fluent builders, no new mental model, onboarding takes minutes.
- Allocation‑aware: messages are captured on demand; zero allocations when data is valid.

## License

MIT — see `LICENSE`.
