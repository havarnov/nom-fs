open System

open System.Buffers
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Configs
open BenchmarkDotNet.Running

open FParsec

open Nom
open Nom.Bytes.Streaming
open NomFs.Core
open NomFs.Bytes.Complete

[<MemoryDiagnoser>]
type TagBenchmark() =
    let mutable inputAsMemory: ReadOnlyMemory<char> = ReadOnlyMemory.Empty

    let mutable inputAsString: string = String.Empty

    let mutable inputAsSequence : ReadOnlySequence<char> = ReadOnlySequence.Empty

    let mutable result: bool = false

    let mutable f: (CharStream<unit> -> Reply<string>) option = None

    let mutable n: (ReadOnlyMemory<char> -> ParseResult<ReadOnlyMemory<char>, ReadOnlyMemory<char>>) option = None

    let mutable c: TagParser<char> option = None

    [<ParamsSource("InputParams")>]
    member val public Input: string * string * bool = (String.Empty, String.Empty, false) with get, set

    member val public InputParams: (string * string * bool) seq =
        seq {
            yield ("{", "{foobar}", true)
            yield ("foobar", "foobar1234", true)
            yield ("1234", "foobar1234", false)
        }

    [<GlobalSetup>]
    member self.GlobalSetupData() =
        let (tagString, inputString, result') = self.Input
        inputAsString <- inputString
        inputAsMemory <- inputAsString.AsMemory()
        inputAsSequence <- ReadOnlySequence(inputAsMemory)
        result <- result'
        f <- Some (pstring tagString)
        n <- Some (tag (m tagString))
        c <- Some (TagParser<char>(ReadOnlySequence(tagString.AsMemory())))
        ()

    [<Benchmark>]
    member self.FParsec() =
        match run f.Value inputAsString with
        | ParserResult.Success (res, _, _) ->
            if result then
                Some res
            else
                raise (Exception "Tag should successfully parse")
        | _ ->
            if not result then
                None
            else
                raise (Exception "Tag should _not_ successfully parse")

    [<Benchmark>]
    member self.NomFs() =
        match n.Value inputAsMemory with
        | Result.Ok res ->
            if result then
                Some res
            else
                raise (Exception "Tag should successfully parse")
        | _ ->
            if not result then
                None
            else
                raise (Exception "Tag should _not_ successfully parse")

    [<Benchmark>]
    member self.NomCs() =
        try
            let res = c.Value.Parse(inputAsSequence)
            Some (res.Item1, res.Item2)
        with
        | :? ParserErrorException ->
            if result then
                raise (Exception "Tag should successfully parse")
            else
                None



[<EntryPoint>]
let main argv =
    let config = DefaultConfig.Instance

    BenchmarkSwitcher.FromTypes([|
        typeof<TagBenchmark>;
        |])
        .Run(argv, config) |> ignore
    0 // return an integer exit code
