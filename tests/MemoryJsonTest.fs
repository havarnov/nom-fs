module NomFs.Memory.Tests.Json

open Xunit

open NomFs.Memory.Bytes.Complete
open NomFs.Memory.Character.Complete
open NomFs.Combinator
open NomFs.Branch
open NomFs.Multi
open NomFs.Memory.Number.Complete
open NomFs.Sequence
open NomFs.Memory.Core

type JsonValue =
    | Str of string
    | Boolean of bool
    | Num of float
    | Array of JsonValue seq
    | Object of Map<string, JsonValue>

let spaceChars = m " \t\r\n"

let sp = takeWhile (fun c -> NomFs.Memory.ReadOnlyMemory.contains c spaceChars)
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
    let j = m """
    
    
    
    
    true


    """
    printfn "%A" (valueParser j)
    0

[<Fact>]
let ``json str test`` () =
    let j = m """
    
    
    
    
    "foobar"


    """
    printfn "%A" (valueParser j)
    0

[<Fact>]
let ``json array test`` () =
    let j = m  """
    
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

    let jsonStr = System.IO.File.ReadAllText(System.IO.Path.Join(System.AppContext.BaseDirectory, "basic.json"));
    printfn "joho: %A" (root (m jsonStr))

    0