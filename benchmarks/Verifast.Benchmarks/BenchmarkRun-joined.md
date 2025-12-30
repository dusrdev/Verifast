```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.1 (25B78) [Darwin 25.1.0]
Apple M2 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 10.0.101
  [Host]    : .NET 10.0.1 (10.0.1, 10.0.125.57005), Arm64 RyuJIT armv8.0-a
  MediumRun : .NET 10.0.1 (10.0.1, 10.0.125.57005), Arm64 RyuJIT armv8.0-a

Job=MediumRun  OutlierMode=RemoveAll  IterationCount=15  
IterationTime=100ms  LaunchCount=2  WarmupCount=10  

```
| Type            | Method           | ValidDto | Mean        | Ratio           | Rank | Allocated | Alloc Ratio   |
|---------------- |----------------- |--------- |------------:|----------------:|-----:|----------:|--------------:|
| AsyncValidation | FluentValidation | False    | 10,935.2 ns |        baseline |    4 |   14104 B |               |
| AsyncValidation | Verifast         | False    |  2,203.2 ns |    4.96x faster |    2 |    1096 B |   12.87x less |
| AsyncValidation | FluentValidation | True     |  8,470.8 ns |        baseline |    3 |    9808 B |               |
| AsyncValidation | Verifast         | True     |  2,101.9 ns |    5.20x faster |    1 |    1016 B |   13.88x less |
|                 |                  |          |             |                 |      |           |               |
| SyncValidation  | FluentValidation | False    | 35,687.7 ns |        baseline |    4 |   63112 B |               |
| SyncValidation  | Verifast         | False    |    141.4 ns | 252.377x faster |    2 |     328 B | 192.415x less |
| SyncValidation  | FluentValidation | True     | 26,091.2 ns |        baseline |    3 |   50063 B |               |
| SyncValidation  | Verifast         | True     |    112.3 ns | 320.264x faster |    1 |         - |            NA |
