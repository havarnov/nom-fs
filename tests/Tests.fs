module NomFs.Tests

open System
open Xunit

open NomFs

let ofOption error = function Some s -> Ok s | None -> Error error

type ResultBuilder() =
    member __.Return(x) = Ok x

    member __.ReturnFrom(m: Result<_, _>) = m

    member __.Bind(m, f) = Result.bind f m
    member __.Bind((m, error): (Option<'T> * 'E), f) = m |> ofOption error |> Result.bind f

    member __.Zero() = None

    member __.Combine(m, f) = Result.bind f m

    member __.Delay(f: unit -> _) = f

    member __.Run(f) = f()

    member __.TryWith(m, h) =
        try __.ReturnFrom(m)
        with e -> h e

    member __.TryFinally(m, compensation) =
        try __.ReturnFrom(m)
        finally compensation()

    member __.Using(res:#IDisposable, body) =
        __.TryFinally(body res, fun () -> match res with null -> () | disp -> disp.Dispose())

    member __.While(guard, f) =
        if not (guard()) then Ok () else
        do f() |> ignore
        __.While(guard, f)

    member __.For(sequence:seq<_>, body) =
        __.Using(sequence.GetEnumerator(), fun enum -> __.While(enum.MoveNext, __.Delay(fun () -> body enum.Current)))

let result = ResultBuilder()

let extractOk res =
    match res with
    | Ok (input, res) -> (input, res)
    | e -> failwithf "Should never happen: %A" e

let extractErr res =
    match res with
    | Error (Err e) -> e
    | e -> failwithf "Should never happen: %A" e

[<Fact>]
let ``alt test`` () =
    let parser input = alt [|digit1;alpha1|] input

    let (input, res) = extractOk (parser "12345abc")
    Assert.Equal("abc", input)
    Assert.Equal("12345", res)

    let (input, res) = extractOk (parser "abc123456")
    Assert.Equal("123456", input)
    Assert.Equal("abc", res)

[<Fact>]
let ``opt test`` () =
    let parser input = opt digit1 input

    let (input, res) = extractOk (parser "12345abc")
    Assert.Equal("abc", input)
    Assert.Equal("12345", res.Value)

    let (input, res) = extractOk (parser "abc123456")
    Assert.Equal("abc123456", input)
    Assert.Equal(None, res)

[<Fact>]
let ``take test`` () =
    let take6 input = take 6 input

    let (input, res) = extractOk (take6 [|1uy;2uy;3uy;4uy;5uy;6uy|])
    Assert.Empty(input)
    Assert.Equal([|1uy;2uy;3uy;4uy;5uy;6uy|], res)

    let (input, res) = extractOk (take6 [|1uy;2uy;3uy;4uy;5uy;6uy;7uy;8uy|])
    Assert.Equal([|7uy;8uy|], input)
    Assert.Equal([|1uy;2uy;3uy;4uy;5uy;6uy|], res)

    let (input, kind) = extractErr (take6 [|1uy;2uy;3uy;4uy;5uy|])
    Assert.Equal<seq<_>>([|1uy;2uy;3uy;4uy;5uy|], input)
    Assert.Equal(Eof, kind)

[<Fact>]
let ``tag test`` () =

    let toStr s = (s |> Seq.toArray |> String)
    let parser = map (tag "Hello") toStr

    let (input, res) = extractOk (parser "Hello, World")
    Assert.Equal(", World", input)
    Assert.Equal("Hello", res)

    let (input, kind) = extractErr (parser "Something")
    Assert.Equal("Something", input)
    Assert.Equal(Tag, kind)

    let (input, kind) = extractErr (parser "")
    Assert.Equal("", input)
    Assert.Equal(Tag, kind)

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

[<Fact>]
let ``map test`` () =
    let parser = map digit1 (fun d -> d |> Seq.length)

    let (input, res) = extractOk (parser "1248abc")
    Assert.Equal("abc", input)
    Assert.Equal(4, res)
    
[<Fact>]
let ``mapRes test`` () =

    let parseByte (str: char seq) =
        try
            Ok (Convert.ToByte (str |> Seq.toArray |> String))
        with
            | e ->
            printfn "%A" e
            Error e.Message

    let parser = mapRes digit1 parseByte

    match parser "123" with
    | Ok (input, b) ->
        Assert.Equal(123uy, b)
        Assert.Equal("", input)
    | e ->
        Assert.False(true, (sprintf "Should never happend: %A" e))

    match parser "abc" with
    | Error (Err (input, Digit)) ->
        Assert.Equal("abc", input)
    | e ->
        Assert.False(true, (sprintf "Should never happend: %A" e))

    match parser "123456" with
    | Error (Err (input, MapRes)) ->
        Assert.Equal("123456", input)
    | e ->
        Assert.False(true, (sprintf "Should never happend: %A" e))

[<Fact>]
let ``takeWhileMN test`` () =

    let parser = takeWhileMN 3 6 Char.IsLetter

    match parser "latin123" with
    | Ok (input, res) ->
        Assert.Equal("latin", res)
        Assert.Equal("123", input)
    | e ->
        Assert.False(true, (sprintf "Should never happend: %A" e))

    match parser "lengthy" with
    | Ok (input, res) ->
        Assert.Equal("length", res)
        Assert.Equal("y", input)
    | e ->
        Assert.False(true, (sprintf "Should never happend: %A" e))

    match parser "latin" with
    | Ok (input, res) ->
        Assert.Equal("latin", res)
        Assert.Equal("", input)
    | e ->
        Assert.False(true, (sprintf "Should never happend: %A" e))

    match parser "ed" with
    | Error (Err (input, TakeWhileMN)) ->
        Assert.Equal("ed", input)
    | e ->
        Assert.False(true, (sprintf "Should never happend: %A" e))

    match parser "12345" with
    | Error (Err (input, TakeWhileMN)) ->
        Assert.Equal("12345", input)
    | e ->
        Assert.False(true, (sprintf "Should never happend: %A" e))

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

type Color = {
    Red: byte;
    Green: byte;
    Blue: byte;
}

[<Fact>]
let ``color test`` () =

    let fromHex i =
        try
            let i = i |> Seq.toArray |> String
            Ok (Convert.ToByte(i, 16))
        with
            | e -> Error e

    let isHexDigit (c: char) =
        "abcdefABCDEF1234567890"
        |> Seq.contains c
   
    let hexPrimary =
        mapRes (takeWhileMN 2 2 isHexDigit) fromHex

    let hexColor input = result {
        let! input, _ = tag "#" input
        let! input, red = hexPrimary input
        let! input, green = hexPrimary input
        let! input, blue = hexPrimary input

        return! Ok (input, { Red = red; Green = green; Blue = blue; })}

    let c = hexColor "#2F14DF"

    match c with
    | Ok (input, co) ->
        Assert.Equal("", input)
        Assert.Equal(47uy, co.Red)
        Assert.Equal(20uy, co.Green)
        Assert.Equal(223uy, co.Blue)
    | e ->
        Assert.False(true, (sprintf "Should never happend: %A" e))
