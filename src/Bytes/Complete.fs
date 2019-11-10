module NomFs.Bytes.Complete

open System

open NomFs.Core

let inline tag (t: ReadOnlyMemory<'a>) : _ -> IResult<ReadOnlyMemory<'a>, ReadOnlyMemory<'a>> =
    let inner (input: ReadOnlyMemory<'a>) =
        if not t.IsEmpty
           && input.Length >= t.Length
           && (t.Span.SequenceEqual(input.Slice(0, t.Length).Span))
        then
            Ok (input.Slice(t.Length), t)
        else
            Error (Err (input, Tag))
    inner

let escaped
    (normal: ReadOnlyMemory<'a> -> IResult<ReadOnlyMemory<'a>, ReadOnlyMemory<'a>>)
    (controlChar: 'a)
    (escapable: ReadOnlyMemory<'a> -> IResult<ReadOnlyMemory<'a>, 'a>) =

    let tryTakeEscaped (input: ReadOnlyMemory<'a>) =
        if input.Length > 0 && input.Span.[0] = controlChar
        then
            escapable (input.Slice(1))
            |> Result.map (fun (inputi, escapedChar) ->
                // inputi, ReadOnlyMemory([|controlChar; escapedChar|]))
                inputi, input.Slice(0, 2))
        else
            Error (Err (input, Escape))

    let rec inner input =
        match normal input with
        | Ok (input, res) when input.IsEmpty || input.Span.[0] <> controlChar ->
            Ok (input, res)
        | Ok (inputi, res) ->
            match tryTakeEscaped inputi with
            | Ok (inputii, escapedSeq) ->
                // let res = concat res escapedSeq
                let res = input.Slice(0, res.Length + escapedSeq.Length)
                inner inputii
                |> Result.map (fun (innerInput, innerRes) ->
                    // (innerInput, concat res innerRes))
                    (innerInput, input.Slice(0, res.Length + innerRes.Length)))
            | Error e -> Error e
        | Error e -> Error e

    inner

let take count =
    let inner (input: ReadOnlyMemory<'a>) =
        if input.Length < count then
            Error (Err (input, Eof))
        else
            Ok (input.Slice(count), input.Slice(0, count))
    inner

let takeWhile predicate =
    let inner (input: ReadOnlyMemory<'a>) =
        let res = NomFs.ReadOnlyMemory.takeWhile predicate input
        Ok (input.Slice(res.Length), res)
    inner

let takeWhileMN m n f =
    let inner (input: ReadOnlyMemory<'a>) =
        let res = input |> NomFs.ReadOnlyMemory.takeWhile f
        if m <= res.Length && res.Length <= n then
            Ok (input.Slice(res.Length), res)
        elif m <= res.Length then
            Ok (input.Slice(n), input.Slice(0, n))
        else
            Error (Err (input, TakeWhileMN))
    inner