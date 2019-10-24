module NomFs.Sequence

open NomFs.Core
open NomFs.Result
open NomFs.Combinator

type private T<'a, 'b> = 'a -> IResult<'a, 'b>

/// Applies a tuple of parsers one by one and returns their results as a tuple.
let tuple2 (t: (T<_,_> * T<_, _>)) =
    let inner input = result {
        let (f, s) = t
        let! input, f = f input
        let! input, s = s input
        return (input, (f, s))}
    inner

/// Applies a tuple of parsers one by one and returns their results as a tuple.
let tuple3 (t: (T<_, _> * T<_, _> * T<_, _>)) =
    let inner input = result {
        let (f, s, t) = t
        let! input, f = f input
        let! input, s = s input
        let! input, t = t input
        return (input, (f, s, t))}
    inner

/// Applies a tuple of parsers one by one and returns their results as a tuple.
let tuple4 (t: (T<_, _> * T<_, _> * T<_, _> * T<_, _>)) =
    let inner input = result {
        let (f, s, t, fo) = t
        let! input, f = f input
        let! input, s = s input
        let! input, t = t input
        let! input, fo = fo input
        return (input, (f, s, t, fo))}
    inner

/// Applies a tuple of parsers one by one and returns their results as a tuple.
let tuple5 (t: (T<_, _> * T<_, _> * T<_, _> * T<_, _> * T<_, _>)) =
    let inner input = result {
        let (f, s, t, fo, fi) = t
        let! input, f = f input
        let! input, s = s input
        let! input, t = t input
        let! input, fo = fo input
        let! input, fi = fi input
        return (input, (f, s, t, fo, fi))}
    inner

/// <summary>
/// Matches an object from the first parser and discards it, then gets an object from the second parser.
/// </summary>
/// <param name="first">The opening parser.</param>
/// <param name="second">The second parser to get object.</param>
let preceded (first: _ -> IResult<_, _>) (second: _ -> IResult<_, _>) =
    let inner input = result {
        let! (input, _) = first input
        return! second input}
    inner

/// <summary>
/// Matches an object from the first parser, then gets an object from the sep_parser,
/// then matches another object from the second parser.
/// </summary>
let delimited first sep second =
    map (tuple3 (first, sep, second)) (fun (_, r, _) -> r)

/// Gets an object from the first parser, then matches an object from the sep_parser and discards it,
/// then gets another object from the second parser.
let separatedPair first sep second =
    map (tuple3 (first, sep, second)) (fun (f, _, s) -> (f, s))