``` ini

BenchmarkDotNet=v0.12.0, OS=macOS 10.15 (19A602) [Darwin 19.0.0]
Intel Core i7-6567U CPU 3.30GHz (Skylake), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=3.0.100
  [Host]     : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), X64 RyuJIT DEBUG
  DefaultJob : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), X64 RyuJIT


```
| Method |        Input |      Mean |    Error |    StdDev |    Median |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------- |------------- |----------:|---------:|----------:|----------:|-------:|------:|------:|----------:|
| **Memory** |         **123;** |  **19.84 ns** | **0.062 ns** |  **0.055 ns** |  **19.84 ns** | **0.0306** |     **-** |     **-** |      **64 B** |
|   Span |         123; | 236.53 ns | 5.247 ns | 15.140 ns | 231.38 ns | 0.1988 |     - |     - |     416 B |
| **Memory** | **Hello, World** |  **20.48 ns** | **0.442 ns** |  **0.454 ns** |  **20.35 ns** | **0.0229** |     **-** |     **-** |      **48 B** |
|   Span | Hello, World | 569.89 ns | 4.289 ns |  3.581 ns | 569.19 ns | 0.3748 |     - |     - |     784 B |
