module NomFs.Tests.Character.Complete

open Xunit

open NomFs.Core
open NomFs.Character.Complete

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
