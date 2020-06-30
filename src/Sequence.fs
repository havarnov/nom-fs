module NomFs.Sequence

open NomFs.Core
open NomFs.Result
open NomFs.Combinator

/// Applies a tuple of parsers one by one and returns their results as a tuple.
let inline tuple2 (t: (Parser<_,_> * Parser<_, _>)) =
    let inline inner input = result {
        let (f, s) = t
        let! input, f = f input
        let! input, s = s input
        return (input, (f, s))}
    inner

/// Applies a tuple of parsers one by one and returns their results as a tuple.
let inline tuple3 (t: (Parser<_, _> * Parser<_, _> * Parser<_, _>)) =
    let inline inner input = result {
        let (f, s, t) = t
        let! input, f = f input
        let! input, s = s input
        let! input, t = t input
        return (input, (f, s, t))}
    inner

/// Applies a tuple of parsers one by one and returns their results as a tuple.
let inline tuple4 (t: (Parser<_, _> * Parser<_, _> * Parser<_, _> * Parser<_, _>)) =
    let inline inner input = result {
        let (f, s, t, fo) = t
        let! input, f = f input
        let! input, s = s input
        let! input, t = t input
        let! input, fo = fo input
        return (input, (f, s, t, fo))}
    inner

/// Applies a tuple of parsers one by one and returns their results as a tuple.
let inline tuple5 (t: (Parser<_, _> * Parser<_, _> * Parser<_, _> * Parser<_, _> * Parser<_, _>)) =
    let inline inner input = result {
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
/// <param name="first">Parserhe opening parser.</param>
/// <param name="second">Parserhe second parser to get object.</param>
let inline preceded first second =
    map (tuple2 (first, second)) (fun (_, s) -> s)

/// Gets an object from the first parser, then matches an object from the second parser and discards it.
let inline terminated first second =
    map (tuple2 (first, second)) (fun (f, _) -> f)

/// <summary>
/// Matches an object from the first parser, then gets an object from the sep_parser,
/// then matches another object from the second parser.
/// </summary>
let inline delimited first sep second =
    map (tuple3 (first, sep, second)) (fun (_, r, _) -> r)

/// Gets an object from the first parser, then matches an object from the sep_parser and discards it,
/// then gets another object from the second parser.
let inline separatedPair first sep second =
    map (tuple3 (first, sep, second)) (fun (f, _, s) -> (f, s))