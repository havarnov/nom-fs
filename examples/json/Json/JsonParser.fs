module NomFs.Examples.Json

open NomFs.Bytes.Complete
open NomFs.Character.Complete
open NomFs.Combinator
open NomFs.Branch
open NomFs.Multi
open NomFs.Number.Complete
open NomFs.Sequence
open NomFs.Core

type JsonValue =
    | Str of string
    | Boolean of bool
    | Num of float
    | Array of JsonValue seq
    | Object of Map<string, JsonValue>

let spaceChars = m " \t\r\n"

let sp = takeWhile (fun c -> NomFs.ReadOnlyMemory.contains c spaceChars)
let psp p = preceded sp p

let strParser = escaped alphanumeric1 '\\' (oneOf (m "\"n\\"))

let stringParser =
    let bs = tag (m "\"")
    map (tuple3 (bs, strParser, bs)) (fun (_, str, _) -> str |> System.String.Concat)

let booleanParser =
    alt
        [
            map (tag (m "true")) (fun _ -> true);
            map (tag (m "false")) (fun _ -> false);
        ]

let rec arrayParser input =
    delimited
        (tag (m "["))
        (separatedList (psp (tag (m ","))) valueParser)
        (tag (m "]"))
        input

and keyValueParser input =
    map
        (tuple3
            (
                (psp stringParser),
                (psp (tag (m ":"))),
                (psp valueParser)))
        (fun (key, _, value) -> (key, value))
        input

and hashParser =
    map
        (delimited (tag (m "{")) (separatedList (psp (tag (m ","))) keyValueParser) (tag (m "}")))
        Map.ofSeq

and valueParser input =
    psp (
        alt
            [
                map hashParser Object;
                map arrayParser Array;
                map stringParser Str;
                map double Num;
                map booleanParser Boolean;
            ])
        input

let parser =
    delimited
        sp
        (
            alt
                [
                    map hashParser Object;
                    map arrayParser Array;
                ]
        )
        sp