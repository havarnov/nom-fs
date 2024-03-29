module NomFs.Bytes.Complete

open System

open NomFs.Core

let inline tag (t: ReadOnlyMemory<'a>) : _ -> ParseResult<ReadOnlyMemory<'a>, ReadOnlyMemory<'a>> =
    let inline inner (input: ReadOnlyMemory<'a>) =
        match NomFs.Bytes.Streaming.tag t input with
        | Error (Incomplete _) ->
            Error (Err (input, Tag))
        | o -> o
    inner

let inline escaped
    (normal: ReadOnlyMemory<'a> -> ParseResult<ReadOnlyMemory<'a>, ReadOnlyMemory<'a>>)
    (controlChar: 'a)
    (escapable: ReadOnlyMemory<'a> -> ParseResult<ReadOnlyMemory<'a>, 'a>) =

    let inline tryTakeEscaped (input: ReadOnlyMemory<'a>) =
        if input.Length > 0 && input.Span.[0] = controlChar
        then
            escapable (input.Slice(1))
            |> Result.map (fun (input', _) ->
                input', input.Slice(0, 2))
        else
            Error (Err (input, Escape))

    let rec inner input =
        match normal input with
        | Ok (input, res) when input.IsEmpty || input.Span.[0] <> controlChar ->
            Ok (input, res)
        | Ok (input', res) ->
            match tryTakeEscaped input' with
            | Ok (input'', escapedSeq) ->
                inner input''
                |> Result.map (fun (innerInput, innerRes) ->
                    (innerInput, input.Slice(0, res.Length + escapedSeq.Length + innerRes.Length)))
            | Error e -> Error e
        | Error e -> Error e

    inner

let inline take count =
    let inline inner (input: ReadOnlyMemory<'a>) =
        if input.Length < count then
            Error (Err (input, Eof))
        else
            Ok (input.Slice(count), input.Slice(0, count))
    inner

let inline takeWhile predicate =
    let inline inner (input: ReadOnlyMemory<'a>) =
        let res = NomFs.ReadOnlyMemory.takeWhile predicate input
        Ok (input.Slice(res.Length), res)
    inner

let inline takeWhileMN m n f =
    let inline inner (input: ReadOnlyMemory<'a>) =
        let res = input |> NomFs.ReadOnlyMemory.takeWhile f
        if m <= res.Length && res.Length <= n then
            Ok (input.Slice(res.Length), res)
        elif m <= res.Length then
            Ok (input.Slice(n), input.Slice(0, n))
        else
            Error (Err (input, TakeWhileMN))
    inner