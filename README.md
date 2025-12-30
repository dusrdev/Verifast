# Verifast

[![NuGet](https://img.shields.io/nuget/v/Verifast.svg?style=flat-square)](https://www.nuget.org/packages/Verifast)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Verifast?style=flat&label=Downloads)](https://www.nuget.org/packages/Verifast)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-net9.0%20%7C%20netstandard2.1%20%7C%20netstandard2.0-512BD4?style=flat-square)](#)

High‑performance, allocation‑friendly validation for .NET 9 and netstandard (2.1/2.0). No complicated APIs, no expression trees - just plain C#. Implement a tiny interface, add errors or warnings, and you’re done.

## Why Verifast

- Minimal API: implement `IValidator<T>` or `IAsyncValidator<T>` and write ordinary C#.
- Fast by design: struct based `ValidationResult<TMessage>` allocates only when you add messages.
- Ergonomic helpers: call `.Validate(...)` for a result or `.TryValidate(...)` for a quick bool.
- Plays well with `ref struct`: APIs are designed to enable stack‑only scenarios.
- Your messages, your way: use `string` or a custom `TMessage` for richer metadata.

## Benchmarks

[BenchmarkDotNet](https://benchmarkdotnet.org/) results comparing `Verifast` to [FluentValidation](https://github.com/FluentValidation/FluentValidation) (baseline, industry standard). Full report: [BenchmarkRun-joined.md](benchmarks/Verifast.Benchmarks/BenchmarkRun-joined.md).

| Type            | Method           | ValidDto | Mean        | Ratio           | Allocated | Alloc Ratio   |
|---------------- |----------------- |--------- |------------:|----------------:|----------:|--------------:|
| AsyncValidation | FluentValidation | False    | 10,935.2 ns |        baseline |   14104 B |               |
| AsyncValidation | Verifast         | False    |  2,203.2 ns |    4.96x faster |    1096 B |   12.87x less |
| AsyncValidation | FluentValidation | True     |  8,470.8 ns |        baseline |    9808 B |               |
| AsyncValidation | Verifast         | True     |  2,101.9 ns |    5.20x faster |    1016 B |   13.88x less |
|                 |                  |          |             |                 |           |               |
| SyncValidation  | FluentValidation | False    | 35,687.7 ns |        baseline |   63112 B |               |
| SyncValidation  | Verifast         | False    |    141.4 ns | 252.377x faster |     328 B | 192.415x less |
| SyncValidation  | FluentValidation | True     | 26,091.2 ns |        baseline |   50063 B |               |
| SyncValidation  | Verifast         | True     |    112.3 ns | 320.264x faster |         - |            NA |

## Install

```bash
dotnet add package Verifast
```

## Quick Start

Define your model and implement its validator. Then call the extension helpers.

```csharp
using Verifast;

public readonly record struct User(string? Name, int Age);

public readonly struct UserValidator : IValidator<User> {
    public void Validate(in User instance, ref ValidationResult<string> result) {
        if (string.IsNullOrWhiteSpace(instance.Name))
            result.AddError("'Name' must be non-empty");
        if (instance.Age is < 18 or > 120)
            result.AddError("'Age' must be between 18 and 120");
    }
}

var user = new User("Ada", 33);
var validator = new UserValidator();
var result = validator.Validate(user);
if (!result.IsValid)
    foreach (var error in result.Errors) Console.WriteLine(error);
```

Or get a simple pass/fail while still capturing details:

```csharp
if (!validator.TryValidate(user, out var result))
    foreach (var error in result.Errors) Console.WriteLine(error);
```

## Async Validation

Prefer async? Implement `IAsyncValidator<T>` and return a `ValueTask<ValidationResult<TMessage>>`.

```csharp
public record AsyncUser(string? Email);

public sealed class AsyncUserValidator : IAsyncValidator<AsyncUser> {
    public async ValueTask<ValidationResult<string>> ValidateAsync(AsyncUser instance, CancellationToken ct = default) {
        await Task.Yield(); // e.g., call a store or service

        ValidationResult<string> result = default;
        if (!string.IsNullOrWhiteSpace(instance.Email))
            if (!LooksLikeEmail(instance.Email!))
                result.AddError("'Email' must be a valid email");

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

public readonly struct EvenValidator : IValidator<int, Msg> {
    public void Validate(in int value, ref ValidationResult<Msg> result) {
        if ((value & 1) != 0)
            result.AddError(new Msg("NotEven", "Value must be even"));
    }
}

if (!new EvenValidator().TryValidate(3, out var res))
    foreach (var e in res.Errors) Console.WriteLine($"{e.Code}: {e.Text}");
```

## Design Philosophy

- Pure language constructs: write straightforward `if`/`for` statements - no fluent builders, no new mental model, onboarding takes minutes.
- Allocation‑aware: messages are captured on demand; zero allocations when data is valid.

## License

MIT - see `LICENSE`.
