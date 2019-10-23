module NomFs.Tests.Number.Complete

open Xunit

open NomFs.Core
open NomFs.Number.Complete

open NomFs.Tests.Core

[<Fact>]
let ``double test`` () =
    let (input, res) = extractOk (double "1.1")
    Assert.Equal("", input)
    Assert.Equal(1.1, res)

    let (input, res) = extractOk (double "123E-02")
    Assert.Equal("", input)
    Assert.Equal(1.23, res)

    let (input, res) = extractOk (double "123K-01")
    Assert.Equal("K-01", input)
    Assert.Equal(123., res)

    let (input, kind) = extractErr (double "abc")
    Assert.Equal("abc", input)
    Assert.Equal(Float, kind)
