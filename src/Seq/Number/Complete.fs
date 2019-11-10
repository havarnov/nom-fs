module NomFs.Number.Complete

open NomFs.Core
open NomFs.Result
open NomFs.Combinator
open NomFs.Branch
open NomFs.Bytes.Complete
open NomFs.Character.Complete
open NomFs.Sequence

let private dotAndAfter input: IResult<_, _> = result {
    let! (input, dot) = opt (tag ".") input
    match dot with
    | Some dot ->
        let! (input, afterDot) = opt digit1 input
        match afterDot with
        | Some afterDot -> return (input, Seq.append dot afterDot)
        | None -> return (input, dot)
    | None -> return (input, Seq.empty)}

let private normalFloat input = result {
    let! (input, (d1, rest)) = tuple2 (digit1, opt dotAndAfter) input
    match rest with
    | Some rest -> return (input, Seq.append d1 rest)
    | None -> return (input, d1)}

let private startWithDot input = result {
    let! (input, (dot, rest)) = tuple2 (tag ".", digit1) input
    return (input, Seq.append dot rest)}

let private e = alt [tag "e"; tag "E"]

let private sign = opt (alt [tag "+"; tag "-"])

let private exp input = result {
    let! (input, (e, sign, d)) = tuple3 (e, sign, digit1) input
    match sign with
    | Some sign ->
        return (input, Seq.append (Seq.append e sign) d)
    | None ->
        return (input, Seq.append e d)}

let private doubleSeq input = result {
    let d = alt [ normalFloat; startWithDot; ]
    let! (input, (sign, d, exp)) = tuple3 (sign, d, opt exp) input
    match (sign, exp) with
    | (Some sign, None) ->
        return (input, Seq.append d sign)
    | (Some sign, Some exp) ->
        return (input, Seq.append (Seq.append sign d) exp)
    | (None, Some exp) ->
        return (input, Seq.append d exp)
    | (None, None) ->
        return (input, d)}

/// Recognizes floating point number in a byte string and returns a f64
let double =
    let inner input = result {
        match doubleSeq input with
        | Ok (input, doubleSeq) ->
            match doubleSeq |> (System.String.Concat >> System.Double.TryParse) with
            | (true, d) -> return (input, d)
            | (false, _) -> return! Error (Err (input, Float))
        | Error _ ->
            return! Error (Err (input, Float))}
    inner