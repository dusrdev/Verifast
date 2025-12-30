# AGENTS.md

This file provides guidance to agents when working with code in this repository.

## Project Overview
- Tech: Multi‑targeted .NET library (`src/Verifast`) targeting `net9.0`, `netstandard2.1`, and `netstandard2.0`, focused on fast, allocation‑aware validation. Unit tests live in `tests/Verifast.Tests.Unit` (net9.0) using xUnit v3 with the Microsoft Testing Platform, and `tests/Verifast.Tests.Unit.Standard` (net8.0) using xUnit v2 to exercise the `netstandard2.0` target. A minimal benchmarks project exists under `benchmarks/Verifast.Benchmarks` (net9.0).
- Design: The library centers on simple, interface‑driven validators and a lightweight result type that captures errors and warnings only when needed.

## Big‑Picture Architecture
- Validation contracts:
  - `IValidator<T, TMessage>` and `IValidator<T>`: synchronous validators. On `net9.0`, both use `where T : allows ref struct` so validators can be used with stack‑only types without forcing `ref struct` everywhere. On `netstandard2.1`/`netstandard2.0`, the constraint is omitted.
  - `IAsyncValidator<T, TMessage>` and `IAsyncValidator<T>`: asynchronous validators returning `ValueTask<ValidationResult<...>>` across targets. The `netstandard2.0` forms are declared with `in T` variance.
- Orchestrator APIs:
  - Static `Validator` class: extension methods for synchronous validation and `TryValidate` overloads. Only sync helpers exist (no async orchestrator today). On `net9.0` the extensions include `allows ref struct` constraints.
- Result type:
  - `ValidationResult<TMessage>`: a struct that tracks `IsValid`, `Errors`, and `Warnings`. Message lists are allocated on‑demand when the first message is added; `Errors`/`Warnings` return `ReadOnlyCollection<TMessage>` wrappers.

## Usage Model
- Implement `IValidator<T>` (or `IAsyncValidator<T>`) on the type being validated or in a separate validator type.
- Sync validators populate results via `Validate(in T instance, ref ValidationResult<...> result)`.
- Execute via `validator.Validate(instance)` / `validator.TryValidate(instance, out var result)` or `validator.ValidateAsync(instance, ct)`.
- Choose `string` messages for simplicity or a custom `TMessage` type for structured metadata.

## Repository Layout
- `src/Verifast`: Library code (`IValidator`, `IAsyncValidator`, `Validator`, `ValidationResult`).
- `tests/Verifast.Tests.Unit`: xUnit v3 tests referencing the library. Configured as `OutputType Exe` to support Microsoft Testing Platform.
- `tests/Verifast.Tests.Unit.Standard`: xUnit v2 tests targeting `net8.0` so the library resolves to its `netstandard2.0` target.
- `benchmarks/Verifast.Benchmarks`: BenchmarkDotNet project (scaffolded).
- `Verifast.slnx`: Solution file exists, but project‑scoped commands are preferred.

## Commands You’ll Commonly Use
Note: Favor project‑scoped commands (operate on `src/Verifast` or `tests/Verifast.Tests.Unit` explicitly). Avoid explicit `dotnet build` before running tests; `dotnet run` builds implicitly.

- Build the library (project‑scoped):
  - `dotnet build src/Verifast/Verifast.csproj`

- Format/lint (EditorConfig conventions assumed):
  - Analyze only: `dotnet format analyze --severity info`
  - Apply style/whitespace fixes: `dotnet format`
  - If `dotnet format` isn’t installed: `dotnet tool update -g dotnet-format`

- Run all tests (Microsoft Testing Platform via dotnet run):
  - From repo root: `dotnet run --project tests/Verifast.Tests.Unit`

- Run netstandard2.0 coverage tests (xUnit v2 via dotnet test):
  - `dotnet test tests/Verifast.Tests.Unit.Standard`

- List tests (Microsoft Testing Platform semantics):
  - `dotnet run --project tests/Verifast.Tests.Unit -- --list-tests`

- Run a single test (recommended: filter by fully qualified name):
  - By method: `dotnet run --project tests/Verifast.Tests.Unit --filter-method="*TestMethodPattern*"`
  - By class: `dotnet run --project tests/Verifast.Tests.Unit --filter-class="*TestClassPattern*"`
  - Alternative (traditional): `dotnet test tests/Verifast.Tests.Unit --filter "FullyQualifiedName~Pattern"`

- Run benchmarks (Release, no debugger):
  - `dotnet run --project benchmarks/Verifast.Benchmarks -c Release`

## Style and Conventions
- C# 10+ idioms; file‑scoped namespaces.
- Braces required around blocks.
- Private fields: `_camelCase`; private static fields: `s_camelCase`.
- Interfaces start with `I`; type parameters start with `T`.
- Prefer explicit, straightforward logic (no heavy reflection or expression trees). Keep hot paths allocation‑aware.

## Test Stack Specifics
- xUnit v3 with `Microsoft.NET.Test.Sdk`. The test project sets `OutputType Exe` and includes `xunit.runner.json`.
- Filters and test listing use Microsoft Testing Platform semantics when using `dotnet run`.
- You can also use `dotnet test` with similar filter semantics if preferred.

## AOT/Trimming Notes
- `IsAotCompatible` and `IsTrimmable` are enabled for the `net9.0` target. Avoid patterns that rely on runtime code generation or deep reflection without proper annotations.

## Notes for Future Agents
- The `allows ref struct` constraints are intentional to enable stack‑only scenarios. Avoid introducing APIs that disallow `ref struct` usage unless necessary.
- Keep the API surface minimal and allocation‑friendly. `ValidationResult<TMessage>` should remain a lightweight struct that only allocates when messages are added.
- Prefer project‑scoped commands; the solution file exists but isn’t required for everyday operations.
