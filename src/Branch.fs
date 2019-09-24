module NomFs.Branch

open NomFs.Core

let alt (parsers: (_ -> IResult<_, _>) seq) =
    let inner input =
        parsers
        |> Seq.fold
            (fun s n ->
                match s with
                | Ok s -> Ok s
                | Error _ -> n input)
            (Error (Err (input, Eof)))
    inner
