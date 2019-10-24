module NomFs.Tests.Sequence

open Xunit

open NomFs.Core
open NomFs.Bytes.Complete
open NomFs.Sequence
open NomFs.Tests.Core

[<Fact>]
let ``test terminated`` () =
    let parser = terminated (tag "abc") (tag "efg")

    let (input, res) = extractOk (parser "abcefg")
    Assert.Equal("", input)
    Assert.Equal("abc", res)

    let (input, res) = extractOk (parser "abcefghij")
    Assert.Equal("hij", input)
    Assert.Equal("abc", res)

    let (input, kind) = extractErr (parser "")
    Assert.Equal("", input)
    Assert.Equal(Tag, kind)

    let (input, kind) = extractErr (parser "123")
    Assert.Equal("123", input)
    Assert.Equal(Tag, kind)

[<Fact>]
let ``test separatedPair`` () =
    let parser = separatedPair (tag "abc") (tag "|") (tag "efg")

    let (input, res) = extractOk (parser "abc|efg")
    Assert.Equal("", input)
    let (f, s) = res
    Assert.Equal("abc", f)
    Assert.Equal("efg", s)

    let (input, res) = extractOk (parser "abc|efghij")
    Assert.Equal("hij", input)
    let (f, s) = res
    Assert.Equal("abc", f)
    Assert.Equal("efg", s)

    let (input, kind) = extractErr (parser "")
    Assert.Equal("", input)
    Assert.Equal(Tag, kind)

    let (input, kind) = extractErr (parser "123")
    Assert.Equal("123", input)
    Assert.Equal(Tag, kind)

[<Fact>]
let ``test delimted`` () =
    let parser = delimited (tag "abc") (tag "|") (tag "efg")

    let (input, res) = extractOk (parser "abc|efg")
    Assert.Equal("", input)
    Assert.Equal("|", res)

    let (input, res) = extractOk (parser "abc|efghij")
    Assert.Equal("hij", input)
    Assert.Equal("|", res)

    let (input, kind) = extractErr (parser "")
    Assert.Equal("", input)
    Assert.Equal(Tag, kind)

    let (input, kind) = extractErr (parser "123")
    Assert.Equal("123", input)
    Assert.Equal(Tag, kind)
