module NomFs.Memory.Character.Complete

open System

open NomFs.Core

open NomFs.Memory.ReadOnlyMemory

let digit1 (input: ReadOnlyMemory<_>): IResult<ReadOnlyMemory<_>, ReadOnlyMemory<_>> =
    let res =
        input
        |> takeWhile Char.IsDigit
    if res.Length > 0
    then
        let rest =
            input.Slice(res.Length)
        Ok (rest, res)
    else
        Error (Err (input, Digit))

let oneOf list =
    let inner (input: ReadOnlyMemory<_>) =
        if input.Length > 0 && contains (input.Span.[0]) list
        then
            Ok (input.Slice(1), input.Span.[0])
        else
            Error (Err (input, OneOf))

    inner

let alphanumeric1 =
    let inline inner (input: ReadOnlyMemory<_>) =
        let res = input |> takeWhile Char.IsLetterOrDigit 
        if res.IsEmpty then
            Error (Err (input, Alphanumeric))
        else
            Ok (input.Slice(res.Length), res)
    inner

let alpha1 =
    let inline inner (input: ReadOnlyMemory<_>) =
        let res = input |> takeWhile Char.IsLetter
        if res.IsEmpty then
            Error (Err (input, Alphanumeric))
        else
            Ok (input.Slice(res.Length), res)
    inner
