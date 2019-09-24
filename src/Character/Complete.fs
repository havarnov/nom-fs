module NomFs.Character.Complete

open System

open NomFs.Core

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