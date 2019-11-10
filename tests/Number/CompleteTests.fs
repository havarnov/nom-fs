module NomFs.Tests.Number.Complete

open System

open Xunit

open NomFs.Core
open NomFs.ReadOnlyMemory
open NomFs.Number.Complete

open NomFs.Tests.Core

[<Fact>]
let ``double test`` () =
    let (input, res) = extractOk (double (m ("1.1")))
    Assert.True(sequenceEqual ReadOnlyMemory.Empty input)
    Assert.Equal(1.1, res)

    let (input, res) = extractOk (double (m "123E-02"))
    Assert.True(sequenceEqual ReadOnlyMemory.Empty input)
    Assert.Equal(1.23, res)

    let (input, res) = extractOk (double (m "123K-01"))
    Assert.True(sequenceEqual (m "K-01") input)
    Assert.Equal(123., res)

    let (input, kind) = extractErr (double (m "abc"))
    Assert.True(sequenceEqual (m "abc") input)
    Assert.Equal(Float, kind)
