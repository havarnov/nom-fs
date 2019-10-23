module NomFs.Tests.Json

open Xunit

open NomFs.Bytes.Complete
open NomFs.Character.Complete
open NomFs.Combinator
open NomFs.Branch
open NomFs.Result
open NomFs.Multi
open NomFs.Core
open NomFs.Number.Complete

type JsonValue =
    | Str of string
    | Boolean of bool
    | Num of float
    | Array of JsonValue seq
    | Object of Map<string, JsonValue>

let spaceChars = " \t\r\n" :> char seq

let sp input = takeWhile (fun c -> Seq.contains c spaceChars) input

let preceded (p: _ -> IResult<_, _>) (f: _ -> IResult<_, _>) input = result {
    let! (input, _) = p input
    return! f input}

let strParser input = escaped alphanumeric1 '\\' (oneOf "\"n\\") input

let stringParser input = result {
    let! (input, _) = tag "\"" input
    let! (input, str) = strParser input
    let! (input, _) = tag "\"" input
    return (input, str |> System.String.Concat)
}

let booleanParser input = alt
                            (seq {
                                yield map (tag("true")) (fun _ -> true);
                                yield map (tag("false")) (fun _ -> false); })
                            input

let rec arrayParser input = result {
    let! (input, _) = tag "[" input
    let! (input, values) = separatedList (preceded sp (tag ",")) valueParser input
    let! (input, _) = tag "]" input
    return (input, values)}

and keyValueParser input = result {
    let! (input, key) = preceded sp stringParser input
    let! (input, _) = preceded sp (tag ":") input
    let! (input, value) = preceded sp valueParser input
    return (input, (key, value))}

and hashParser input = result {
    let! (input, _) = tag "{" input
    let! (input, values) = separatedList (preceded sp (tag ",")) keyValueParser input
    let! (input, _) = tag "}" input
    return (input, Map.ofSeq values)}

and valueParser input = result {
    let i = preceded
                sp
                (alt
                    (seq {
                        yield map hashParser Object;
                        yield map arrayParser Array;
                        yield map stringParser Str;
                        yield map double Num;
                        yield map booleanParser Boolean;
                    }))
    return! i input}

let root input = result {
    let! (input, _) = sp input
    let! (input, res) =
        alt
            (seq {
                yield map arrayParser Array;
            })
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