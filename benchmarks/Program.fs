// Learn more about F# at http://fsharp.org

open System
open System.IO
open System.Collections.Concurrent

open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running

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
    member self.Span() = spanParser (self.Input) 

[<EntryPoint>]
let main argv =
    BenchmarkRunner.Run<TagComparison>() |> ignore
    0 // return an integer exit code
