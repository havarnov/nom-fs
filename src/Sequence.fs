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