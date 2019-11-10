module NomFs.Tests.Combinator

open Xunit

open NomFs.Combinator
open NomFs.Character.Complete
open NomFs.Tests.Core
open System
open NomFs.Core
open NomFs.ReadOnlyMemory

[<Fact>]
let ``opt test`` () =
    let parser input = opt digit1 input

    let (input, res) = extractOk (parser (m "12345abc"))
    Assert.True(sequenceEqual (m "abc") input)
    Assert.True(sequenceEqual (m "12345") res.Value)

    let (input, res) = extractOk (parser (m "abc123456"))
    Assert.True(sequenceEqual (m "abc123456") input)
    Assert.Equal(None, res)

[<Fact>]
let ``map test`` () =
    let parser = map digit1 (fun d -> d.Length)

    let (input, res) = extractOk (parser (m "1248abc"))
    Assert.True(sequenceEqual (m "abc") input)
    Assert.Equal(4, res)
    
[<Fact>]
let ``mapRes test`` () =

    let parseByte (str: ReadOnlyMemory<char>) =
        try
            Ok (Convert.ToByte (str.ToString()))
        with
        | e ->
            Error e.Message

    let parser = mapRes digit1 parseByte

    let (input, res) = extractOk (parser (m "123"))
    Assert.True(sequenceEqual ReadOnlyMemory.Empty input)
    Assert.Equal(123uy, res)

    let (input, kind) = extractErr (parser (m "abc"))
    Assert.True(sequenceEqual (m "abc") input)
    Assert.Equal(Digit, kind)

    let (input, kind) = extractErr (parser (m "123456"))
    Assert.True(sequenceEqual (m "123456") input)
    Assert.Equal(MapRes, kind)
