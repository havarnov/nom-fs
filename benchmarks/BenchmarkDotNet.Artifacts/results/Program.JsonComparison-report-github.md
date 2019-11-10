``` ini

BenchmarkDotNet=v0.12.0, OS=macOS 10.15 (19A602) [Darwin 19.0.0]
Intel Core i7-6567U CPU 3.30GHz (Skylake), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=3.0.100
  [Host]     : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), X64 RyuJIT DEBUG
  DefaultJob : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), X64 RyuJIT


```
| Method |      Input |     Mean |    Error |   StdDev |   Gen 0 | Gen 1 | Gen 2 | Allocated |
|------- |----------- |---------:|---------:|---------:|--------:|------:|------:|----------:|
| Memory | basic.json | 39.08 us | 0.690 us | 0.612 us | 26.3672 |     - |     - |  53.95 KB |
