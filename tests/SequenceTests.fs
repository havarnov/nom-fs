module NomFs.Tests.Sequence

open Xunit

open NomFs.Bytes.Complete
open NomFs.Sequence

let ``test tuple2`` () =
    let t = tag "foo"
    let parser = tuple2 (t, t)

    match parser "foofoo" with 
    | Ok (input, (t1, t2)) ->
        Assert.Equal("", input)
        Assert.Equal("foo", t1)
        Assert.Equal("foo", t2)
    | e ->
        Assert.False(true, (sprintf "Should never happend: %A" e))