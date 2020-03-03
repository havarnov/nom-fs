module NomFs.Tests.Bytes.Streaming

open System

open Xunit

open NomFs.Combinator
open NomFs.Core
open NomFs.Bytes.Streaming
open NomFs.ReadOnlyMemory
open NomFs.Tests.Core

[<Fact>]
let ``tag test`` () =

    let parser = map (tag (m "Hello")) (fun r -> r.ToString())

    let (input, res) = extractOk (parser (m "Hello, World"))
    Assert.True(sequenceEqual (m ", World") input)
    Assert.Equal("Hello", res)

    let (input, kind) = extractErr (parser (m "Something"))
    Assert.True(sequenceEqual (m "Something") input)
    Assert.Equal(Tag, kind)

    let expectedLength = extractIncomplete (parser (m ""))
    Assert.Equal(5, expectedLength)

    let expectedLength = extractIncomplete (parser (m "Hell"))
    Assert.Equal(5, expectedLength)
