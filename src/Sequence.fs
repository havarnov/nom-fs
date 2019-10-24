module NomFs.Sequence

open NomFs.Core
open NomFs.Result

let tuple2 (t: ('a -> IResult<'a, 'b>) * ('a -> IResult<'a, 'c>)) =
    let inner input = result {
        let (f, s) = t
        let! input, f = f input
        let! input, s = s input
        return (input, (f, s))}
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