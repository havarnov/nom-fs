module NomFs

open System

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

type Needed =
    | Unknown
    | Size of uint32

type Err<'E> =
    | Incomplete of int
    | Err of 'E
    | Failure of 'E

type ErrorKind =
    | Tag
    | Digit
    | MapRes
    | TakeWhileMN
    | Tuple
    | Eof

type IResult<'I, 'O> = Result<'I * 'O, Err<'I * ErrorKind>>

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

let opt p input =
    match p input with
    | Ok (rest, res) -> Ok (rest, Some res)
    | Error (Err (rest, _)) -> Ok (rest, None)

let alt (parsers: (_ -> IResult<_, _>) seq) =
    let inner input =
        parsers
        |> Seq.fold
            (fun s n ->
                match s with
                | Ok s -> Ok s
                | Error _ -> n input)
            (Error (Err (input, Eof)))
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

let mapRes f s =
    let inner i =
        match f i with
        | Ok (input, o1) ->
            match s o1 with
            | Ok o2 -> Ok (input, o2)
            | Error _ -> Error (Err (i, MapRes))
        | Error e -> Error e
    inner

let map f s =
    let inner i =
        match f i with
        | Ok (input, o1) -> Ok (input, s o1)
        | Error e -> Error e
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


let tuple2 (t: ('a -> IResult<'a, 'b>) * ('a -> IResult<'a, 'c>)) =
    let inner input = result {
        let (f, s) = t
        let! input, f = f input
        let! input, s = s input
        return (input, (f, s))}

    inner
