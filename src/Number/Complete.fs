module NomFs.Number.Complete

open NomFs.Core
open NomFs.Result
open NomFs.Combinator
open NomFs.Branch
open NomFs.Bytes.Complete
open NomFs.Character.Complete

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
    let! (input, d1) = digit1 input
    let! (input, rest) = opt dotAndAfter input
    match rest with
    | Some rest -> return (input, Seq.append d1 rest)
    | None -> return (input, d1)}

let private startWithDot input = result {
    let! (input, dot) = tag "." input
    let! (input, rest) = digit1 input
    return (input, Seq.append dot rest)}

let private exp input = result {
    let! (input, e) = alt (seq { yield tag "e"; yield tag "E"; }) input
    let! (input, sign) = opt (alt (seq { yield tag "+"; yield tag "-" })) input
    let! (input, d) = digit1 input
    match sign with
    | Some sign ->
        return (input, Seq.append (Seq.append e sign) d)
    | None ->
        return (input, Seq.append e d)}

let private doubleSeq input : IResult<_, _> = result {
    let! (input, sign) = opt (alt (seq { yield tag "+"; yield tag "-" })) input
    let! (input, d) = alt (seq { yield normalFloat; yield startWithDot; }) input
    let! (input, exp) = opt exp input
    match (sign, exp) with
    | (Some sign, None) ->
        return (input, Seq.append d sign)
    | (Some sign, Some exp) ->
        return (input, Seq.append (Seq.append sign d) exp)
    | (None, Some exp) ->
        return (input, Seq.append d exp)
    | (None, None) ->
        return (input, d)}

let double input = result {
    match doubleSeq input with
    | Ok (input, doubleSeq) ->
        match doubleSeq |> (System.String.Concat >> System.Double.TryParse) with
        | (true, d) -> return (input, d)
        | (false, _) -> return! Error (Err (input, Float))
    | Error _ ->
        return! Error (Err (input, Float))}