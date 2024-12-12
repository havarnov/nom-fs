module NomFs.Number.Complete

open System

open System.Globalization
open NomFs.Core
open NomFs.Result
open NomFs.Combinator
open NomFs.Branch
open NomFs.Bytes.Complete
open NomFs.Character.Complete
open NomFs.Sequence

let inline private dotAndAfter (input: ReadOnlyMemory<_>): ParseResult<_, _> = result {
    let! (inputi, dot) = opt (tag (m ".")) input
    match dot with
    | Some dot ->
        let! (inputii, afterDot) = opt digit1 inputi
        match afterDot with
        | Some afterDot -> return (inputii, input.Slice(0, dot.Length + afterDot.Length))
        | None -> return (inputii, dot)
    | None -> return (inputi, ReadOnlyMemory.Empty)}

let inline private normalFloat (input: ReadOnlyMemory<_>) = result {
    let! (inputi, (d1, rest)) = tuple2 (digit1, opt dotAndAfter) input
    match rest with
    | Some rest -> return (inputi, input.Slice(0, d1.Length + rest.Length))
    | None -> return (inputi, d1)}

let inline private startWithDot (input: ReadOnlyMemory<_>) = result {
    let! (inputi, (dot, rest)) = tuple2 (tag (m "."), digit1) input
    return (inputi, input.Slice(0, dot.Length + rest.Length))}

let private e = alt [tag (m "e"); tag (m "E")]

let private sign = opt (alt [tag (m "+"); tag (m "-")])

let inline private exp (input: ReadOnlyMemory<_>) = result {
    let! (input', (e, sign, d)) = tuple3 (e, sign, digit1) input
    match sign with
    | Some sign ->
        return (input', input.Slice(0, e.Length + sign.Length + d.Length))
    | None ->
        return (input', input.Slice(0, e.Length + d.Length))}

let inline private doubleSeq (input: ReadOnlyMemory<_>) = result {
    let d = alt [ normalFloat; startWithDot; ]
    let! (input', (sign, d, exp)) = tuple3 (sign, d, opt exp) input
    match (sign, exp) with
    | (Some sign, None) ->
        return (input', input.Slice(0, d.Length + sign.Length))
    | (Some sign, Some exp) ->
        return (input', input.Slice(0, sign.Length + d.Length + exp.Length))
    | (None, Some exp) ->
        return (input', input.Slice(0, d.Length + exp.Length))
    | (None, None) ->
        return (input', d)}

/// Recognizes floating point number in a byte string and returns a f64
let double =
    let inline inner (input: ReadOnlyMemory<_>) = result {
        match doubleSeq input with
        | Ok (input, doubleSeq) ->
            // match doubleSeq |> (System.String.Concat >> System.Double.TryParse) with
            match Double.TryParse(doubleSeq |> String.Concat, NumberFormatInfo.InvariantInfo) with
            | (true, d) -> return (input, d)
            | (false, _) -> return! Error (Err (input, Float))
        | Error _ ->
            return! Error (Err (input, Float))}
    inner