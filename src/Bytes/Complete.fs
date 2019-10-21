module NomFs.Bytes.Complete

open NomFs.Core

let escaped
    (normal: seq<char> -> IResult<seq<char>, seq<char>>)
    (controlChar: char)
    (escapable: seq<char> -> IResult<seq<char>, char>) =
    let tryTakeEscaped (input: seq<char>) =
        let c = Seq.head input
        if c = controlChar
        then
            escapable (input |> Seq.skip 1)
            |> Result.map (fun (input, escapedChar) ->
                input, seq { yield controlChar; yield escapedChar; })
        else
            Error (Err (input, Escape))

    let rec inner input =
        match normal input with
        | Ok (input, res) when Seq.isEmpty input || (Seq.head input) <> controlChar ->
            Ok (input, res)
        | Ok (input, res) ->
            match tryTakeEscaped input with
            | Ok (input, escapedSeq) ->
                let res = Seq.append res escapedSeq
                inner input
                |> Result.map (fun (innerInput, innerRes) ->
                    (innerInput, Seq.append res innerRes))
            | Error e -> Error e
        | Error e -> Error e

    inner

let tag t =
    let inner (input: 'T seq) : IResult<_, _> =
        if input |> Seq.length < 1
        then
            Error (Err (input, Tag))
        elif
            input
            |> Seq.zip t
            |> Seq.forall (fun (x, y) -> x = y)
        then
            let rest =
                input
                |> Seq.skip (Seq.length t)
            Ok (rest, t)
        else
            Error (Err (input, Tag))
    inner

let take count =
    let inner input =
        if input |> Seq.length < count then
            Error (Err (input, Eof))
        else
            let res = input |> Seq.take count
            let rest = input |>Seq.skip count
            Ok (rest, res)
    inner

let takeWhile predicate : (_ -> IResult<_, _>) =
    let inner input =
        let res =
            input
            |> Seq.takeWhile predicate
        let rest =
            input
            |> Seq.skipWhile predicate
        Ok (rest, res)
    inner

let takeWhileMN m n f : ('a -> IResult<'a, 'a>) =
    let inner input =
        let res =
            input
            |> Seq.takeWhile f
        let l = res |> Seq.length
        if m <= l && l <= n
        then
            let rest =
                input
                |> Seq.skip (Seq.length res)
            Ok (rest, res)
        elif m <= l
        then
            let res =
                input
                |> Seq.take n
            let rest =
                input
                |> Seq.skip n
            Ok (rest, res)
        else
            Error (Err (input, TakeWhileMN))
    inner