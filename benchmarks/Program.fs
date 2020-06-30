open System
open System.IO

open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running

open BenchmarkDotNet.Configs;
open NomFs.Core
open FParsec.CharParsers

[<MemoryDiagnoser>]
type JsonStringComparison () =
    let mutable inputAsMemory: ReadOnlyMemory<char> = ReadOnlyMemory.Empty
    let mutable inputAsString: string = String.Empty

    [<Params ("\"string\"")>]
    member val public Input = String.Empty with get, set

    [<GlobalSetup>]
    member self.GlobalSetupData() =
        inputAsMemory <- (self.Input.AsMemory())
        inputAsString <- self.Input
        ()

    [<Benchmark>]
    member self.Fparsec() =
        match FParsec.CharParsers.run Parser.jstring inputAsString with
        | ParserResult.Success (res, _, _) -> res
        | _ -> raise (Exception "Every json should parse")

    [<Benchmark>]
    member self.Memory() =
        match NomFs.Examples.Json.jstring inputAsMemory with
        | Ok res -> res
        | _ -> raise (Exception "Every json should parse")

[<MemoryDiagnoser>]
type JsonValueComparison () =
    let mutable inputAsMemory: ReadOnlyMemory<char> = ReadOnlyMemory.Empty
    let mutable inputAsString: string = String.Empty

    [<Params ("null", "\"string\"", "123.32", "false")>]
    member val public Input = String.Empty with get, set

    [<GlobalSetup>]
    member self.GlobalSetupData() =
        inputAsMemory <- (self.Input.AsMemory())
        inputAsString <- self.Input
        ()

    [<Benchmark>]
    member self.Fparsec() =
        match Parser.parseJsonString inputAsString with
        | ParserResult.Success (res, _, _) -> res
        | _ -> raise (Exception "Every json should parse")

    [<Benchmark>]
    member self.Memory() =
        match NomFs.Examples.Json.valueParser inputAsMemory with
        | Ok res -> res
        | _ -> raise (Exception "Every json should parse")

[<MemoryDiagnoser>]
type JsonComparison () =
    let mutable inputAsMemory: ReadOnlyMemory<char> = ReadOnlyMemory.Empty
    let mutable inputAsString: string = String.Empty

    [<Params ("string.json", "number.json", "bool.json", "basic.json", "large.json")>]
    member val public Input = String.Empty with get, set

    [<GlobalSetup>]
    member self.GlobalSetupData() =
        let jsonStr = File.ReadAllText(Path.Join(AppContext.BaseDirectory, "testfiles", self.Input));
        inputAsMemory <- (jsonStr.AsMemory())
        inputAsString <- jsonStr
        ()

    [<Benchmark>]
    member self.Fparsec() =
        match Parser.parseJsonString inputAsString with
        | ParserResult.Success (res, _, _) -> res
        | _ -> raise (Exception "Every json should parse")

    [<Benchmark>]
    member self.Memory() =
        match NomFs.Examples.Json.parser inputAsMemory with
        | Ok res -> res
        | _ -> raise (Exception "Every json should parse")

[<EntryPoint>]
let main argv =
    // due to unoptimized build of fparseccs
    let config = DefaultConfig.Instance.With(ConfigOptions.DisableOptimizationsValidator)

    BenchmarkSwitcher.FromTypes([|
        typeof<JsonComparison>;
        typeof<JsonValueComparison>;
        typeof<JsonStringComparison>;
        |])
        .Run(argv, config) |> ignore
    0 // return an integer exit code
