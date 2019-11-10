module NomFs.Tests.Sequence

open Xunit

open NomFs.Core
open NomFs.Bytes.Complete
open NomFs.Sequence
open NomFs.Tests.Core
open NomFs.ReadOnlyMemory
open System

[<Fact>]
let ``test terminated`` () =
    let parser = terminated (tag (m "abc")) (tag (m "efg"))

    let (input, res) = extractOk (parser (m "abcefg"))
    Assert.True(sequenceEqual ReadOnlyMemory.Empty input)
    Assert.True(sequenceEqual (m "abc") res)

    let (input, res) = extractOk (parser (m "abcefghij"))
    Assert.True(sequenceEqual (m "hij") input)
    Assert.True(sequenceEqual (m "abc") res)

    let (input, kind) = extractErr (parser (m ""))
    Assert.True(sequenceEqual ReadOnlyMemory.Empty input)
    Assert.Equal(Tag, kind)

    let (input, kind) = extractErr (parser (m "123"))
    Assert.True(sequenceEqual (m "123") input)
    Assert.Equal(Tag, kind)

[<Fact>]
let ``test separatedPair`` () =
    let parser = separatedPair (tag (m "abc")) (tag (m "|")) (tag (m "efg"))

    let (input, res) = extractOk (parser (m "abc|efg"))
    Assert.True(sequenceEqual ReadOnlyMemory.Empty input)
    let (f, s) = res
    Assert.True(sequenceEqual (m "abc") f)
    Assert.True(sequenceEqual (m "efg") s)

    let (input, res) = extractOk (parser (m "abc|efghij"))
    Assert.True(sequenceEqual (m "hij") input)
    let (f, s) = res
    Assert.True(sequenceEqual (m "abc") f)
    Assert.True(sequenceEqual (m "efg") s)

    let (input, kind) = extractErr (parser (m ""))
    Assert.True(sequenceEqual ReadOnlyMemory.Empty input)
    Assert.Equal(Tag, kind)

    let (input, kind) = extractErr (parser (m "123"))
    Assert.True(sequenceEqual (m "123") input)
    Assert.Equal(Tag, kind)

[<Fact>]
let ``test delimted`` () =
    let parser = delimited (tag (m "abc")) (tag (m "|")) (tag (m "efg"))

    let (input, res) = extractOk (parser (m "abc|efg"))
    Assert.True(sequenceEqual ReadOnlyMemory.Empty input)
    Assert.True(sequenceEqual (m "|") res)

    let (input, res) = extractOk (parser (m "abc|efghij"))
    Assert.True(sequenceEqual (m "hij") input)
    Assert.True(sequenceEqual (m "|") res)

    let (input, kind) = extractErr (parser (m ""))
    Assert.True(sequenceEqual ReadOnlyMemory.Empty input)
    Assert.Equal(Tag, kind)

    let (input, kind) = extractErr (parser (m "123"))
    Assert.True(sequenceEqual (m "123") input)
    Assert.Equal(Tag, kind)
