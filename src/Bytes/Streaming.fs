module NomFs.Bytes.Streaming

open System

open NomFs.Core

let inline tag (t: ReadOnlyMemory<'a>) : _ -> IResult<ReadOnlyMemory<'a>, ReadOnlyMemory<'a>> =
    let inline inner (input: ReadOnlyMemory<'a>) =
        let isEmptyOrToShort =
            t.IsEmpty || input.Length < t.Length
        if not isEmptyOrToShort
           && (t.Span.SequenceEqual(input.Slice(0, t.Length).Span))
        then
            Ok (input.Slice(t.Length), t)
        elif isEmptyOrToShort
        then
            Error (Incomplete t.Length)
        else
            Error (Err (input, Tag))
    inner
