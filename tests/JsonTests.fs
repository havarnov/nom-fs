module NomFs.Tests.Json

open Xunit

open NomFs.Bytes.Complete
open NomFs.Character.Complete
open NomFs.Combinator
open NomFs.Branch
open NomFs.Multi
open NomFs.Number.Complete
open NomFs.Sequence

type JsonValue =
    | Str of string
    | Boolean of bool
    | Num of float
    | Array of JsonValue seq
    | Object of Map<string, JsonValue>

let spaceChars = " \t\r\n" :> char seq

let sp = takeWhile (fun c -> Seq.contains c spaceChars)
let psp p = preceded sp p

let strParser = escaped alphanumeric1 '\\' (oneOf "\"n\\")

let stringParser =
    let bs = tag "\""
    map (tuple3 (bs, strParser, bs)) (fun (_, str, _) -> str |> System.String.Concat)

let booleanParser =
    alt
        [
            map (tag("true")) (fun _ -> true);
            map (tag("false")) (fun _ -> false);
        ]

let delimited pre parser post =
    map (tuple3 (pre, parser, post)) (fun (_, r, _) -> r)

let rec arrayParser input =
    delimited
        (tag "[")
        (separatedList (psp (tag ",")) valueParser)
        (tag "]")
        input

and keyValueParser input =
    map
        (tuple3
            (
                (psp stringParser),
                (psp (tag ":")),
                (psp valueParser)))
        (fun (key, _, value) -> (key, value))
        input

and hashParser =
    map
        (delimited (tag "{") (separatedList (psp (tag ",")) keyValueParser) (tag "}"))
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

let root =
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

[<Fact>]
let ``json true test`` () =
    let j = """
    
    
    
    
    true


    """
    printfn "%A" (valueParser j)
    0

[<Fact>]
let ``json str test`` () =
    let j = """
    
    
    
    
    "foobar"


    """
    printfn "%A" (valueParser j)
    0

[<Fact>]
let ``json array test`` () =
    let j = """
    
    [

        "foo",

        {
            "ball": true,
            "snall": "hav",
            "ja": 1234.123e-12,
            "nei": 1234.123E+12
        },

        false

    ]


    """
    printfn "%A" (root j)
    0