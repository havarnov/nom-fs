module NomFs.Bytes.Complete

open NomFs.Core

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