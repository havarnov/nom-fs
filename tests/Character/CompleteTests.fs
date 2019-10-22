module NomFs.Tests.Character.Complete

open Xunit

open NomFs.Core
open NomFs.Character.Complete

open NomFs.Tests.Core

[<Fact>]
let ``alphanumeric test`` () =
    let (input, res) = extractOk (alphanumeric1 "21cZ%1")
    Assert.Equal("%1", input)
    Assert.Equal("21cZ", res)

    let (input, kind) = extractErr (alphanumeric1 "&H")
    Assert.Equal("&H", input)
    Assert.Equal(Alphanumeric, kind)

    let (input, kind) = extractErr (alphanumeric1 "")
    Assert.Equal("", input)
    Assert.Equal(Alphanumeric, kind)

[<Fact>]
let ``oneof test`` () =
    let (input, res) = extractOk (oneOf "abc" "b")
    Assert.Equal("", input)
    Assert.Equal('b', res)

    let (input, kind) = extractErr (oneOf "a" "bc")
    Assert.Equal("bc", input)
    Assert.Equal(OneOf, kind)

    let (input, kind) = extractErr (oneOf "a" "")
    Assert.Equal("", input)
    Assert.Equal(OneOf, kind)

[<Fact>]
let ``digit test`` () =

    let parser i = digit1 i

    match parser "21c" with
    | Ok (input, d) ->
        Assert.Equal("21", d)
        Assert.Equal("c", input)
    | _ -> 
        Assert.False(true, "Should never happend")

    match parser "c1" with
    | Error (Err (input, Digit)) ->
        Assert.Equal("c1", input)
    | _ -> 
        Assert.False(true, "Should never happend")

    match parser "" with
    | Error (Err (input, Digit)) ->
        Assert.Equal("", input)
    | _ -> 
        Assert.False(true, "Should never happend")
