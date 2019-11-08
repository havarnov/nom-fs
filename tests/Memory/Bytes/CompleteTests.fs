module NomFs.Tests.Memory.Bytes.Complete

open System

open Xunit

open NomFs.Core
open NomFs.Memory.Bytes.Complete
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

module ReadOnlyMemory =
    let takeWhile (predicate: 'a -> bool) (source: ReadOnlyMemory<'a>) =
        let mutable splitAt = None
        let mutable current = 0
        while Option.isNone splitAt do
            if not (predicate source.Span.[current])
            then
                splitAt <- Some current
            
            current <- current + 1
        
        match splitAt with
        | None -> source
        | Some splitAt ->
            source.Slice(0, splitAt)
    
    let contains (value: 'a) (source: ReadOnlyMemory<'a>) =
        let mutable i = 0
        let mutable found = false
        while i < source.Length && not found do
            if source.Span.[i] = value
            then
                found <- true
            i <- i + 1
        found


let digit1 (input: ReadOnlyMemory<_>): IResult<ReadOnlyMemory<_>, ReadOnlyMemory<_>> =
    let res =
        input
        |> ReadOnlyMemory.takeWhile Char.IsDigit
    if res.Length > 0
    then
        let rest =
            input.Slice(res.Length)
        Ok (rest, res)
    else
        Error (Err (input, Digit))

let oneOf list =
    let inner (input: ReadOnlyMemory<_>) =
        if input.Length > 0 && ReadOnlyMemory.contains (input.Span.[0]) list
        then
            Ok (input.Slice(1), input.Span.[0])
        else
            Error (Err (input, OneOf))

    inner

[<Fact>]
let ``escaped test`` () =
    let parser = escaped digit1 '\\' (oneOf (m @"""n\"))

    let (input, res) = extractOk (parser (m "123;"))
    Assert.Equal(";", input.ToString())
    Assert.Equal("123", res.ToString())

    let (input, res) = extractOk (parser (m "12\\\"34;"))
    Assert.Equal(";", input.ToString())
    Assert.Equal("12\\\"34", res.ToString())
