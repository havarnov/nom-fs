module NomFs.Bytes.Memory.Complete

open System

open NomFs.Core

let tag (t: ReadOnlyMemory<'a>) : _ -> IResult<ReadOnlyMemory<'a>, ReadOnlyMemory<'a>> =
    let inner (input: ReadOnlyMemory<'a>) =
        if not t.IsEmpty
           && input.Length >= t.Length
           && (t.Span.SequenceEqual(input.Slice(0, t.Length).Span))
        then
            Ok (input.Slice(t.Length), t)
        else
            Error (Err (input, Tag))
    inner

// let insertAt0 (a: 'a) (b: ReadOnlyMemory<'a>) =
//     let buffer = (Array.zeroCreate (b.Length + 1))

//     buffer.[0] <- a
//     for i in 0..b.Length do
//         buffer.[i + 1] <- b.Span.[i]

//     ReadOnlyMemory(buffer)

let concat (a: ReadOnlyMemory<'a>) (b: ReadOnlyMemory<'a>) =
    let buffer = (Array.zeroCreate (a.Length + b.Length))

    for i in 0..(a.Length - 1) do
        buffer.[i] <- a.Span.[i]

    for i in 0..(b.Length - 1) do
        buffer.[i + a.Length] <- b.Span.[i]

    ReadOnlyMemory(buffer)

let escaped
    (normal: ReadOnlyMemory<'a> -> IResult<ReadOnlyMemory<'a>, ReadOnlyMemory<'a>>)
    (controlChar: 'a)
    (escapable: ReadOnlyMemory<'a> -> IResult<ReadOnlyMemory<'a>, 'a>) =

    let tryTakeEscaped (input: ReadOnlyMemory<'a>) =
        if input.Length > 0 && input.Span.[0] = controlChar
        then
            escapable (input.Slice(1))
            |> Result.map (fun (input, escapedChar) ->
                input, ReadOnlyMemory([|controlChar; escapedChar|]))
        else
            Error (Err (input, Escape))

    let rec inner input =
        match normal input with
        | Ok (input, res) when input.IsEmpty || input.Span.[0] <> controlChar ->
            Ok (input, res)
        | Ok (input, res) ->
            match tryTakeEscaped input with
            | Ok (input, escapedSeq) ->
                let res = concat res escapedSeq
                inner input
                |> Result.map (fun (innerInput, innerRes) ->
                    (innerInput, concat res innerRes))
            | Error e -> Error e
        | Error e -> Error e

    inner
