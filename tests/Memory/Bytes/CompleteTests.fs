module NomFs.Tests.Memory.Bytes.Complete

open System

open Xunit

open NomFs.Core
open NomFs.Memory.Bytes.Complete
open NomFs.Memory.Character.Complete
open NomFs.Tests.Core
open NomFs.Memory.Core

[<Fact>]
let ``test tag`` () =
    let parser = tag ("Hello".AsMemory())

    let (input, res) = extractOk (parser ("Hello, World".AsMemory()))
    Assert.True(", World".AsMemory().Span.SequenceEqual(input.Span))
    Assert.True("Hello".AsMemory().Span.SequenceEqual(res.Span))

    let (input, kind) = extractErr (parser (m "Something"))
    Assert.Equal(m "Something", input)
    Assert.Equal(Tag, kind)

    let (input, kind) = extractErr (parser (m ""))
    Assert.Equal(m "", input)
    Assert.Equal(Tag, kind)

[<Fact>]
let ``escaped test`` () =
    let parser = escaped digit1 '\\' (oneOf (m @"""n\"))

    let (input, res) = extractOk (parser (m "123;"))
    Assert.Equal(";", input.ToString())
    Assert.Equal("123", res.ToString())

    let (input, res) = extractOk (parser (m "12\\\"34;"))
    Assert.Equal(";", input.ToString())
    Assert.Equal("12\\\"34", res.ToString())
