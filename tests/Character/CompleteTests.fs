module NomFs.Tests.Character.Complete

open Xunit

open NomFs.Core
open NomFs.Character.Complete
open NomFs.ReadOnlyMemory

open NomFs.Tests.Core
open System

[<Fact>]
let ``alphanumeric test`` () =
    let (input, res) = extractOk (alphanumeric1 (m "21cZ%1"))
    Assert.True(sequenceEqual (m "%1") input)
    Assert.True(sequenceEqual (m "21cZ") res)

    let (input, kind) = extractErr (alphanumeric1 (m "&H"))
    Assert.True(sequenceEqual (m "&H") input)
    Assert.Equal(Alphanumeric, kind)

    let (input, kind) = extractErr (alphanumeric1 (m ""))
    Assert.True(sequenceEqual ReadOnlyMemory.Empty input)
    Assert.Equal(Alphanumeric, kind)

[<Fact>]
let ``oneof test`` () =
    let (input, res) = extractOk (oneOf (m "abc") (m "b"))
    Assert.True(sequenceEqual ReadOnlyMemory.Empty input)
    Assert.Equal('b', res)

    let (input, kind) = extractErr (oneOf (m "a") (m "bc"))
    Assert.True(sequenceEqual (m "bc") input)
    Assert.Equal(OneOf, kind)

    let (input, kind) = extractErr (oneOf (m "a") (m ""))
    Assert.True(sequenceEqual ReadOnlyMemory.Empty input)
    Assert.Equal(OneOf, kind)

[<Fact>]
let ``digit test`` () =

    let parser i = digit1 i

    let (input, res) = extractOk (parser (m "21c"))
    Assert.True(sequenceEqual (m "c") input)
    Assert.True(sequenceEqual (m "21") res)

    let (input, kind) = extractErr (parser (m "c1"))
    Assert.True(sequenceEqual (m "c1") input)
    Assert.Equal(Digit, kind)

    let (input, kind) = extractErr (parser (m ""))
    Assert.True(sequenceEqual ReadOnlyMemory.Empty input)
    Assert.Equal(Digit, kind)
