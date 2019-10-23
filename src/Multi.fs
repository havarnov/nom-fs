module NomFs.Multi

open NomFs.Core
open NomFs.Result

let separatedList (sep: _ -> IResult<_, _>) (f: _ -> IResult<_, _>) input =
    let rec inner input res = result {
        match sep input with
        | Error (Err (input, _)) ->
            return! Ok (input, res)
        | Ok (input, _) ->
            match f input with
            | Ok (i1, o) ->
                if i1 = input then
                    return! Error (Err (input, SeparatedList))
                else
                    return! inner i1 (Seq.append res (Seq.singleton o))
            | Error _ -> return! Ok (input, res)
    }

    match f input with
    | Ok (i1, o) ->
        if i1 = input then
            Error (Err (input, Many))
        else
            inner i1 (Seq.singleton o)
    | Error _ -> Ok (input, Seq.empty)

let many0 (parser: _ -> IResult<_, _>) input =
    let rec inner input res =
        match parser input with
        | Ok (i1, o) ->
            if i1 = input then
                Error (Err (input, Many))
            else
                inner i1 (Seq.append res (Seq.singleton o))
        | Error _ -> Ok (input, res)
    
    inner input Seq.empty
