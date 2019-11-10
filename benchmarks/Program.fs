// Learn more about F# at http://fsharp.org

open System
open System.IO
open System.Collections.Concurrent

open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running

[<MemoryDiagnoser>]
type JsonComparison () =
    let mutable inputAsMemory: ReadOnlyMemory<char> = ReadOnlyMemory.Empty
    let mutable inputAsString: string = String.Empty

    [<Params ("basic.json", "large.json")>] 
    member val public Input = String.Empty with get, set

    [<GlobalSetup>]
    member self.GlobalSetupData() =
        let jsonStr = File.ReadAllText(Path.Join(AppContext.BaseDirectory, "testfiles", self.Input));
        inputAsString <- jsonStr
        inputAsMemory <- (jsonStr.AsMemory())
        ()

    [<Benchmark>]
    member self.Memory() = NomFs.Memory.Tests.Json.root inputAsMemory

    [<Benchmark>]
    member self.Seq() = NomFs.Tests.Json.root (self.Input) 

[<MemoryDiagnoser>]
type TagComparison () =
    
    let mutable inputAsMemory: ReadOnlyMemory<char> = ReadOnlyMemory.Empty
    let memoryParser = NomFs.Memory.Bytes.Complete.tag ("Hello".AsMemory())
    let spanParser = NomFs.Bytes.Complete.tag "Hello"

    [<Params ("Hello, World", "123;")>] 
    member val public Input = String.Empty with get, set

    [<GlobalSetup>]
    member self.GlobalSetupData() =
        inputAsMemory <- (self.Input.AsMemory())
        ()

    [<Benchmark>]
    member self.Memory() = memoryParser inputAsMemory

    [<Benchmark>]
    member self.Seq() = spanParser (self.Input) 

[<EntryPoint>]
let main argv =
    BenchmarkSwitcher.FromTypes([|
        typeof<TagComparison>;
        typeof<JsonComparison>
        |]).Run(argv) |> ignore
    0 // return an integer exit code
