module NomFs.Branch

open NomFs.Core

let alt (parsers: (_ -> IResult<_, _>) seq) =
    let inline folder input s n =
        match s with
        | Error _ -> n input
        | res -> res

    let inner input =
        let folder' = folder input
        parsers
        |> Seq.fold
            folder'
            (Error (Err (input, Eof)))
    inner
