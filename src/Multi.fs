module NomFs.Multi

open NomFs.Core
open NomFs.Result

/// <summary>
/// Alternates between two parsers to produce a list of elements.
/// </summary>
/// <param name="sep">Parses the separator between list elements.</param>
/// <param name="f">Parses the elements of the list.</param>
let inline separatedList sep f =
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
            | Error (Err _) -> return! Ok (input, res)
            | Error _ -> return! Error (Failure (input, SeparatedList))
        | Error _ -> return! Error (Failure (input, SeparatedList))}

    let inline outer input =
        match f input with
        | Ok (i1, o) ->
            if i1 = input then
                Error (Err (input, Many))
            else
                inner i1 (Seq.singleton o)
        | Error (Err _) -> Ok (input, Seq.empty)
        | Error _ -> Error (Failure (input, SeparatedList))

    outer

let inline many0 (parser: _ -> ParseResult<_, _>) input =
    let rec inner input res =
        match parser input with
        | Ok (i1, o) ->
            if i1 = input then
                Error (Err (input, Many))
            else
                inner i1 (Seq.append res (Seq.singleton o))
        | Error _ -> Ok (input, res)

    inner input Seq.empty
