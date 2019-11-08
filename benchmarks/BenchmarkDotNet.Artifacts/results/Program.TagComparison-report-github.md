``` ini

BenchmarkDotNet=v0.12.0, OS=macOS 10.15 (19A602) [Darwin 19.0.0]
Intel Core i7-6567U CPU 3.30GHz (Skylake), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=3.0.100
  [Host]     : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), X64 RyuJIT DEBUG
  DefaultJob : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), X64 RyuJIT


```
| Method | Input |      Mean |    Error |   StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------- |------ |----------:|---------:|---------:|-------:|------:|------:|----------:|
| Memory |  123; |  19.52 ns | 0.300 ns | 0.281 ns | 0.0306 |     - |     - |      64 B |
|   Span |  123; | 217.43 ns | 2.183 ns | 1.823 ns | 0.1988 |     - |     - |     416 B |
