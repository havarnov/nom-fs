module NomFs.Tests.Branch

open Xunit

open NomFs.Tests.Core
open NomFs.Branch
open NomFs.Character.Complete
open NomFs.Core
open NomFs.ReadOnlyMemory

[<Fact>]
let ``alt test`` () =
    let parser input = alt [|digit1;alpha1|] input

    let (input, res) = extractOk (parser (m "12345abc"))
    Assert.True(sequenceEqual (m "abc") input)
    Assert.True(sequenceEqual (m "12345") res)

    let (input, res) = extractOk (parser (m "abc123456"))
    Assert.True(sequenceEqual (m "123456") input)
    Assert.True(sequenceEqual (m "abc") res)
