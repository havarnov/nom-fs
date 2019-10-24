module NomFs.Tests.Json

open Xunit

open NomFs.Bytes.Complete
open NomFs.Character.Complete
open NomFs.Combinator
open NomFs.Branch
open NomFs.Result
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

let stringParser input = result {
    let! (input, _) = tag "\"" input
    let! (input, str) = strParser input
    let! (input, _) = tag "\"" input
    return (input, str |> System.String.Concat)
}

let booleanParser =
    alt
        [
            map (tag("true")) (fun _ -> true);
            map (tag("false")) (fun _ -> false);
        ]

let rec arrayParser input = result {
    let! (input, _) = tag "[" input
    let! (input, values) = separatedList (psp (tag ",")) valueParser input
    let! (input, _) = tag "]" input
    return (input, values)}

and keyValueParser input = result {
    let! (input, key) = psp stringParser input
    let! (input, _) = psp (tag ":") input
    let! (input, value) = psp valueParser input
    return (input, (key, value))}

and hashParser input = result {
    let! (input, _) = tag "{" input
    let! (input, values) = separatedList (psp (tag ",")) keyValueParser input
    let! (input, _) = tag "}" input
    return (input, Map.ofSeq values)}

and valueParser input = result {
    let i = preceded
                sp
                (alt
                    [
                        map hashParser Object;
                        map arrayParser Array;
                        map stringParser Str;
                        map double Num;
                        map booleanParser Boolean;
                    ])
    return! i input}

let root input = result {
    let! (input, _) = sp input
    let! (input, res) =
        alt
            [
                map hashParser Object;
                map arrayParser Array;
            ]
            input
    let! (input, _) = sp input
    return (input, res)}

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