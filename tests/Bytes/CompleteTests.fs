module NomFs.Tests.Bytes.Complete

open System
open Xunit

open NomFs.Combinator
open NomFs.Core
open NomFs.Bytes.Complete

open NomFs.Tests.Core
open NomFs.Character.Complete

[<Fact>]
let ``escaped test`` () =
    let parser = escaped digit1 '\\' (oneOf @"""n\")

    let (input: char seq, res) = extractOk (parser ("123;"))
    Assert.Equal(";", input)
    Assert.Equal("123", res)

    let (input: char seq, res) = extractOk (parser ("12\\\"34;"))
    Assert.Equal(";", input)
    Assert.Equal("12\\\"34", res)


[<Fact>]
let ``tag test`` () =

    let toStr s = (s |> Seq.toArray |> String)
    let parser = map (tag "Hello") toStr

    let (input, res) = extractOk (parser "Hello, World")
    Assert.Equal(", World", input)
    Assert.Equal("Hello", res)

    let (input, kind) = extractErr (parser "Something")
    Assert.Equal("Something", input)
    Assert.Equal(Tag, kind)

    let (input, kind) = extractErr (parser "")
    Assert.Equal("", input)
    Assert.Equal(Tag, kind)

[<Fact>]
let ``take while test`` () =
    let parser = takeWhile Char.IsLetter

    let (input, res) = extractOk (parser "latin123")
    Assert.Equal("123", input)
    Assert.Equal("latin", res)

    let (input, res) = extractOk (parser "12345")
    Assert.Equal("12345", input)
    Assert.Equal<seq<_>>(Seq.empty, res)

    let (input, res) = extractOk (parser "latin")
    Assert.Equal<seq<_>>(Seq.empty, input)
    Assert.Equal("latin", res)

    let (input, res) = extractOk (parser "")
    Assert.Equal<seq<_>>(Seq.empty, input)
    Assert.Equal<seq<_>>(Seq.empty, res)

[<Fact>]
let ``take test`` () =
    let take6 input = take 6 input

    let (input, res) = extractOk (take6 [|1uy;2uy;3uy;4uy;5uy;6uy|])
    Assert.Empty(input)
    Assert.Equal([|1uy;2uy;3uy;4uy;5uy;6uy|], res)

    let (input, res) = extractOk (take6 [|1uy;2uy;3uy;4uy;5uy;6uy;7uy;8uy|])
    Assert.Equal([|7uy;8uy|], input)
    Assert.Equal([|1uy;2uy;3uy;4uy;5uy;6uy|], res)

    let (input, kind) = extractErr (take6 [|1uy;2uy;3uy;4uy;5uy|])
    Assert.Equal<seq<_>>([|1uy;2uy;3uy;4uy;5uy|], input)
    Assert.Equal(Eof, kind)

[<Fact>]
let ``takeWhileMN test`` () =

    let parser = takeWhileMN 3 6 Char.IsLetter

    match parser "latin123" with
    | Ok (input, res) ->
        Assert.Equal("latin", res)
        Assert.Equal("123", input)
    | e ->
        Assert.False(true, (sprintf "Should never happend: %A" e))

    match parser "lengthy" with
    | Ok (input, res) ->
        Assert.Equal("length", res)
        Assert.Equal("y", input)
    | e ->
        Assert.False(true, (sprintf "Should never happend: %A" e))

    match parser "latin" with
    | Ok (input, res) ->
        Assert.Equal("latin", res)
        Assert.Equal("", input)
    | e ->
        Assert.False(true, (sprintf "Should never happend: %A" e))

    match parser "ed" with
    | Error (Err (input, TakeWhileMN)) ->
        Assert.Equal("ed", input)
    | e ->
        Assert.False(true, (sprintf "Should never happend: %A" e))

    match parser "12345" with
    | Error (Err (input, TakeWhileMN)) ->
        Assert.Equal("12345", input)
    | e ->
        Assert.False(true, (sprintf "Should never happend: %A" e))