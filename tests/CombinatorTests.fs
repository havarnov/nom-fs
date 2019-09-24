module NomFs.Tests.Combinator

open Xunit

open NomFs.Combinator
open NomFs.Character.Complete
open NomFs.Tests.Core
open System
open NomFs.Core

[<Fact>]
let ``opt test`` () =
    let parser input = opt digit1 input

    let (input, res) = extractOk (parser "12345abc")
    Assert.Equal("abc", input)
    Assert.Equal("12345", res.Value)

    let (input, res) = extractOk (parser "abc123456")
    Assert.Equal("abc123456", input)
    Assert.Equal(None, res)

[<Fact>]
let ``map test`` () =
    let parser = map digit1 (fun d -> d |> Seq.length)

    let (input, res) = extractOk (parser "1248abc")
    Assert.Equal("abc", input)
    Assert.Equal(4, res)
    
[<Fact>]
let ``mapRes test`` () =

    let parseByte (str: char seq) =
        try
            Ok (Convert.ToByte (str |> Seq.toArray |> String))
        with
            | e ->
            printfn "%A" e
            Error e.Message

    let parser = mapRes digit1 parseByte

    match parser "123" with
    | Ok (input, b) ->
        Assert.Equal(123uy, b)
        Assert.Equal("", input)
    | e ->
        Assert.False(true, (sprintf "Should never happend: %A" e))

    match parser "abc" with
    | Error (Err (input, Digit)) ->
        Assert.Equal("abc", input)
    | e ->
        Assert.False(true, (sprintf "Should never happend: %A" e))

    match parser "123456" with
    | Error (Err (input, MapRes)) ->
        Assert.Equal("123456", input)
    | e ->
        Assert.False(true, (sprintf "Should never happend: %A" e))
