module NomFs.Character.Complete

open System

open NomFs.Core

let alphanumeric1 input =
    let isAlphanumeric c = Char.IsDigit c || Char.IsLetter c
    let res =
        input
        |> Seq.takeWhile isAlphanumeric
    if res |> Seq.isEmpty
    then
        Error (Err (input, Alphanumeric))
    else
        let rest =
            input
            |> Seq.skip (Seq.length res)
        Ok (rest, res)

let oneOf list : ('a seq -> IResult<'a seq, 'a>) =
    let inner input =
        match Seq.tryHead input with
        | Some head when list |> Seq.contains head ->
            Ok (Seq.tail input, head)
        | _ ->
            Error (Err (input, OneOf))

    inner

let alpha1 input =
    let res =
        input
        |> Seq.takeWhile Char.IsLetter
    if res |> Seq.length > 0
    then
        let rest =
            input
            |> Seq.skip (Seq.length res)
        Ok (rest, res)
    else
        Error (Err (input, Digit))

let digit1 input =
    let res =
        input
        |> Seq.takeWhile Char.IsDigit
    if res |> Seq.length > 0
    then
        let rest =
            input
            |> Seq.skip (Seq.length res)
        Ok (rest, res)
    else
        Error (Err (input, Digit))