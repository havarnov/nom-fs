module NomFs.Combinator

open NomFs.Core
open System

let opt p input =
    match p input with
    | Ok (rest, res) -> Ok (rest, Some res)
    | Error (Err (rest, _)) -> Ok (rest, None)
    | _ -> raise (NotImplementedException "opt")

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

