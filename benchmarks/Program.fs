open System
open System.IO

open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running

open BenchmarkDotNet.Configs;

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
type TagComparison () =
    
    let mutable inputAsMemory: ReadOnlyMemory<char> = ReadOnlyMemory.Empty
    let memoryParser = NomFs.Bytes.Complete.tag ("Hello".AsMemory())

    [<Params ("Hello, World", "123;")>] 
    member val public Input = String.Empty with get, set

    [<GlobalSetup>]
    member self.GlobalSetupData() =
        inputAsMemory <- (self.Input.AsMemory())
        ()

    [<Benchmark>]
    member self.Memory() = memoryParser inputAsMemory

[<EntryPoint>]
let main argv =
    // due to unoptimized build of fparseccs
    let o = DefaultConfig.Instance.With(ConfigOptions.DisableOptimizationsValidator)

    BenchmarkSwitcher.FromTypes([|
        typeof<TagComparison>;
        typeof<JsonComparison>;
        |])
        .Run(argv, o) |> ignore
    0 // return an integer exit code
