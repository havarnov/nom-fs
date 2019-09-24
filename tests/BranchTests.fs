module NomFs.Tests.Branch

open Xunit

open NomFs.Tests.Core
open NomFs.Branch
open NomFs.Character.Complete

[<Fact>]
let ``alt test`` () =
    let parser input = alt [|digit1;alpha1|] input

    let (input, res) = extractOk (parser "12345abc")
    Assert.Equal("abc", input)
    Assert.Equal("12345", res)

    let (input, res) = extractOk (parser "abc123456")
    Assert.Equal("123456", input)
    Assert.Equal("abc", res)
