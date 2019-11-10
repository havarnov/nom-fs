``` ini

BenchmarkDotNet=v0.12.0, OS=macOS 10.15 (19A602) [Darwin 19.0.0]
Intel Core i7-6567U CPU 3.30GHz (Skylake), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=3.0.100
  [Host]     : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), X64 RyuJIT DEBUG
  DefaultJob : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), X64 RyuJIT


```
| Method |        Input |        Mean |     Error |     StdDev |      Median |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------- |------------- |------------:|----------:|-----------:|------------:|-------:|------:|------:|----------:|
| **Memory** |         **123;** |    **19.85 ns** |  **0.356 ns** |   **0.316 ns** |    **19.96 ns** | **0.0306** |     **-** |     **-** |      **64 B** |
|    Seq |         123; |   319.90 ns | 39.365 ns | 116.067 ns |   256.68 ns | 0.1988 |     - |     - |     416 B |
| **Memory** | **Hello, World** |    **37.39 ns** |  **6.042 ns** |  **17.625 ns** |    **30.44 ns** | **0.0229** |     **-** |     **-** |      **48 B** |
|    Seq | Hello, World | 1,367.07 ns | 44.892 ns | 122.132 ns | 1,386.82 ns | 0.3738 |     - |     - |     784 B |
