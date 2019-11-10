// Learn more about F# at http://fsharp.org

open System
open System.IO
open System.Collections.Concurrent

open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running

[<MemoryDiagnoser>]
type JsonComparison () =
    let mutable inputAsMemory: ReadOnlyMemory<char> = ReadOnlyMemory.Empty

    [<Params ("basic.json")>] 
    member val public Input = String.Empty with get, set

    [<GlobalSetup>]
    member self.GlobalSetupData() =
        let jsonStr = File.ReadAllText(Path.Join(AppContext.BaseDirectory, "testfiles", self.Input));
        inputAsMemory <- (jsonStr.AsMemory())
        ()

    [<Benchmark>]
    member self.Memory() = NomFs.Examples.Json.parser inputAsMemory


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
    BenchmarkSwitcher.FromTypes([|
        typeof<TagComparison>;
        typeof<JsonComparison>
        |]).Run(argv) |> ignore
    0 // return an integer exit code
