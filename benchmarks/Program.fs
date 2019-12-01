open System
open System.IO

open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running

open BenchmarkDotNet.Configs;
open NomFs.Core
open FParsec.CharParsers

[<MemoryDiagnoser>]
type JsonComparison () =
    let mutable inputAsMemory: ReadOnlyMemory<char> = ReadOnlyMemory.Empty
    let mutable inputAsString: string = String.Empty

    [<Params ("basic.json")>] 
    member val public Input = String.Empty with get, set

    [<GlobalSetup>]
    member self.GlobalSetupData() =
        let jsonStr = File.ReadAllText(Path.Join(AppContext.BaseDirectory, "testfiles", self.Input));
        inputAsMemory <- (jsonStr.AsMemory())
        inputAsString <- jsonStr
        ()

    [<Benchmark>]
    member self.Fparsec() =
        let result = Parser.parseJsonString inputAsString
        ()

    [<Benchmark>]
    member self.Memory() =
        let result = NomFs.Examples.Json.parser inputAsMemory
        ()

    [<Benchmark>]
    member self.Newtonsoft() =
        let result = Newtonsoft.Json.Linq.JObject.Parse(inputAsString)
        ()

    [<Benchmark>]
    member self.TestJson() =
        let result = System.Text.Json.JsonDocument.Parse(inputAsString)
        ()

[<MemoryDiagnoser>]
type BoolAltComparison () =
    
    let mutable inputAsMemory: ReadOnlyMemory<char> = ReadOnlyMemory.Empty
    let mutable inputAsString: string = String.Empty

    let fparseccTrueParser = stringReturn "true"  (true)
    let fparseccFalseParser = stringReturn "false" (false)
    let fparseccBoolParser = FParsec.Primitives.choice [ fparseccTrueParser; fparseccFalseParser; ]

    let nomTrueParser = NomFs.Combinator.map (NomFs.Bytes.Complete.tag (m "true")) (fun _ -> true);
    let nomFalseParser = NomFs.Combinator.map (NomFs.Bytes.Complete.tag (m "false")) (fun _ -> true);
    let nomBoolParser = NomFs.Branch.alt [ nomTrueParser; nomFalseParser; ]

    [<Params ("true", "false", "somethingelse")>] 
    member val public Input = String.Empty with get, set

    [<GlobalSetup>]
    member self.GlobalSetupData() =
        inputAsMemory <- (self.Input.AsMemory())
        inputAsString <- self.Input
        ()

    [<Benchmark>]
    member self.NomFsMemoryReadOnly() =
        match nomBoolParser inputAsMemory with
        | Ok (_, r) -> Ok r
        | Error (Err (e, _)) -> Error e
        | _ -> raise (Exception "should not happen.")

    [<Benchmark>]
    member self.FparsecPstring() =
        match run fparseccBoolParser inputAsString with
        | ParserResult.Success (s, _, _) -> Ok s
        | ParserResult.Failure (s, _, _) -> Error s

[<MemoryDiagnoser>]
type TagComparison () =
    
    let mutable inputAsMemory: ReadOnlyMemory<char> = ReadOnlyMemory.Empty
    let mutable inputAsString: string = String.Empty
    let memoryParser = NomFs.Bytes.Complete.tag ("Hello".AsMemory())
    let fparseccParser = pstring "Hello"

    [<Params ("Hello, World", "123;")>] 
    member val public Input = String.Empty with get, set

    [<GlobalSetup>]
    member self.GlobalSetupData() =
        inputAsMemory <- (self.Input.AsMemory())
        inputAsString <- self.Input
        ()

    [<Benchmark>]
    member self.NomFsMemoryReadOnly() =
        match memoryParser inputAsMemory with
        | Ok (_, r) -> r
        | Error (Err (e, _)) -> e
        | _ -> raise (Exception "should not happen.")

    [<Benchmark>]
    member self.FparsecPstring() =
        match run fparseccParser inputAsString with
        | ParserResult.Success (s, _, _) -> s
        | ParserResult.Failure (s, _, _) -> s

[<EntryPoint>]
let main argv =
    // due to unoptimized build of fparseccs
    let o = DefaultConfig.Instance.With(ConfigOptions.DisableOptimizationsValidator)

    BenchmarkSwitcher.FromTypes([|
        typeof<TagComparison>;
        typeof<JsonComparison>;
        typeof<BoolAltComparison>;
        |])
        .Run(argv, o) |> ignore
    0 // return an integer exit code
