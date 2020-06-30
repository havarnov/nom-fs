open System

open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Configs
open BenchmarkDotNet.Running

open FParsec

open NomFs.Core
open NomFs.Bytes.Complete

[<MemoryDiagnoser>]
type TagSuccessfulBenchmark() =
    let mutable inputAsMemory: ReadOnlyMemory<char> = ReadOnlyMemory.Empty
    let mutable inputAsString: string = String.Empty

    let fparsecPstringParser = pstring "{"
    let nomfsTagParser = tag (m "{")

    [<Params ("{foo}")>]
    member val public Input = String.Empty with get, set

    [<GlobalSetup>]
    member self.GlobalSetupData() =
        inputAsMemory <- self.Input.AsMemory()
        inputAsString <- self.Input
        ()

    [<Benchmark>]
    member self.FParsec() =
        match run fparsecPstringParser inputAsString with
        | ParserResult.Success (res, _, _) -> res
        | _ -> raise (Exception "Every tag should parse")

    [<Benchmark>]
    member self.NomFs() =
        match nomfsTagParser inputAsMemory with
        | Result.Ok res -> res
        | _ -> raise (Exception "Every tag should parse")

[<EntryPoint>]
let main argv =
    // due to unoptimized build of fparseccs
    let config = DefaultConfig.Instance.With(ConfigOptions.DisableOptimizationsValidator)

    BenchmarkSwitcher.FromTypes([|
        typeof<TagSuccessfulBenchmark>;
        |])
        .Run(argv, config) |> ignore
    0 // return an integer exit code
