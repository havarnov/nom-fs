module NomFs.Tests.Bytes.Complete

open System

open Xunit

open NomFs.Combinator
open NomFs.Core
open NomFs.Bytes.Complete
open NomFs.ReadOnlyMemory
open NomFs.Tests.Core
open NomFs.Character.Complete

[<Fact>]
let ``escaped test`` () =
    let parser = escaped digit1 '\\' (oneOf (m @"""n\"))

    let (input, res) = extractOk (parser (m "123;"))
    Assert.True(sequenceEqual (m ";") input)
    Assert.True(sequenceEqual (m "123") res)

    let (input, res) = extractOk (parser (m "12\\\"34;"))
    Assert.True(sequenceEqual (m ";") input)
    Assert.True(sequenceEqual (m "12\\\"34") res)


[<Fact>]
let ``tag test`` () =

    let parser = map (tag (m "Hello")) (fun r -> r.ToString())

    let (input, res) = extractOk (parser (m "Hello, World"))
    Assert.True(sequenceEqual (m ", World") input)
    Assert.Equal("Hello", res)

    let (input, kind) = extractErr (parser (m "Something"))
    Assert.True(sequenceEqual (m "Something") input)
    Assert.Equal(Tag, kind)

    let (input, kind) = extractErr (parser (m ""))
    Assert.True(sequenceEqual (m "") input)
    Assert.Equal(Tag, kind)

[<Fact>]
let ``take while test`` () =
    let parser = NomFs.Bytes.Complete.takeWhile Char.IsLetter

    let (input, res) = extractOk (parser (m "latin123"))
    Assert.True(sequenceEqual (m "123") input)
    Assert.True(sequenceEqual (m "latin") res)

    let (input, res) = extractOk (parser (m "12345"))
    Assert.True(sequenceEqual (m "12345") input)
    Assert.True(sequenceEqual ReadOnlyMemory.Empty res)

    let (input, res) = extractOk (parser (m "latin"))
    Assert.True(sequenceEqual ReadOnlyMemory.Empty input)
    Assert.True(sequenceEqual (m "latin") res)

    let (input, res) = extractOk (parser (m ""))
    Assert.True(sequenceEqual ReadOnlyMemory.Empty input)
    Assert.True(sequenceEqual ReadOnlyMemory.Empty res)

[<Fact>]
let ``take test`` () =
    let take6 input = take 6 input

    let (input, res) = extractOk (take6 (a [|1uy;2uy;3uy;4uy;5uy;6uy|]))
    Assert.True(sequenceEqual ReadOnlyMemory.Empty input)
    Assert.True(sequenceEqual (ReadOnlyMemory([|1uy;2uy;3uy;4uy;5uy;6uy|])) res)

    let (input, res) = extractOk (take6 (a [|1uy;2uy;3uy;4uy;5uy;6uy;7uy;8uy|]))
    Assert.True(sequenceEqual (ReadOnlyMemory([|7uy;8uy|])) input)
    Assert.True(sequenceEqual (ReadOnlyMemory([|1uy;2uy;3uy;4uy;5uy;6uy|])) res)

    let (input, kind) = extractErr (take6 (a [|1uy;2uy;3uy;4uy;5uy|]))
    Assert.True(sequenceEqual (ReadOnlyMemory([|1uy;2uy;3uy;4uy;5uy|])) input)
    Assert.Equal(Eof, kind)

[<Fact>]
let ``takeWhileMN test`` () =

    let parser = takeWhileMN 3 6 Char.IsLetter

    let (input, res) = extractOk (parser (m "latin123"))
    Assert.True(sequenceEqual (m "123") input)
    Assert.True(sequenceEqual (m "latin") res)

    let (input, res) = extractOk (parser (m "lengthy"))
    Assert.True(sequenceEqual (m "y") input)
    Assert.True(sequenceEqual (m "length") res)

    let (input, res) = extractOk (parser (m "latin"))
    Assert.True(sequenceEqual ReadOnlyMemory.Empty input)
    Assert.True(sequenceEqual (m "latin") res)

    let (input, kind) = extractErr (parser (m "ed"))
    Assert.True(sequenceEqual (m "ed") input)
    Assert.Equal(TakeWhileMN, kind)

    let (input, kind) = extractErr (parser (m "12345"))
    Assert.True(sequenceEqual (m "12345") input)
    Assert.Equal(TakeWhileMN, kind)
