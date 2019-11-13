``` ini

BenchmarkDotNet=v0.12.0, OS=macOS 10.15.1 (19B88) [Darwin 19.0.0]
Intel Core i7-6567U CPU 3.30GHz (Skylake), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=3.0.100
  [Host]     : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), X64 RyuJIT DEBUG
  DefaultJob : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), X64 RyuJIT


```
|     Method |      Input |      Mean |     Error |     StdDev |    Median |   Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------- |----------- |----------:|----------:|-----------:|----------:|--------:|------:|------:|----------:|
|    Fparsec | basic.json |  9.994 us | 0.3620 us |  1.0210 us |  9.741 us |  1.3428 |     - |     - |    2824 B |
|     Memory | basic.json | 47.632 us | 3.6644 us | 10.3356 us | 43.060 us | 26.3672 |     - |     - |   55248 B |
| Newtonsoft | basic.json |  4.839 us | 0.1720 us |  0.4934 us |  4.634 us |  2.7847 |     - |     - |    5832 B |
|   TestJson | basic.json |  1.742 us | 0.0411 us |  0.1172 us |  1.690 us |  0.2518 |     - |     - |     512 B |
