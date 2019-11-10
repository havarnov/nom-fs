module NomFs.Tests.Json

open Xunit

open NomFs.Bytes.Complete
open NomFs.Character.Complete
open NomFs.Combinator
open NomFs.Branch
open NomFs.Multi
open NomFs.Number.Complete
open NomFs.Sequence
open NomFs.Core
open System

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

    match valueParser j with
    | Ok (_, Boolean b) -> Assert.True(b)
    | _ -> Assert.True(false, "Should never happend")

[<Fact>]
let ``json str test`` () =
    let j = m """
    
    
    
    
    "foobar"


    """
    match valueParser j with
    | Ok (_, Str s) -> Assert.Equal("foobar", s)
    | _ -> Assert.True(false, "Should never happend")

[<Fact>]
let ``json array test`` () =
    let j = m  """
    
    [

        "foo",

        {
            "ball": true,
            "snall": "hav",
            "ja":     1234.123e-12,

            "nei": 1234.123E+12
        },

        false

    ]


    """
    match root j with
    | Ok (_, Array a) ->
        Assert.Equal(3, Seq.length a)
        let s = Seq.filter (fun i -> match i with | Str i -> i = "foo" | _ -> false) a
        let _ = Assert.Single(s)
        let b = Seq.filter (fun i -> match i with | Boolean b -> not b | _ -> false) a
        let _ = Assert.Single(b)
        let o = Seq.filter (fun i -> match i with | Object _ -> true | _ -> false) a
        let o = match Assert.Single(o) with | Object o -> o | _ -> raise (Exception "should never happen")
        Assert.Equal(Boolean true, o.Item "ball")
        Assert.Equal(Str "hav", o.Item "snall")
        Assert.Equal(Num 1234.123e-12, o.Item "ja")
        Assert.Equal(Num 1234.123e+12, o.Item "nei")
    | _ -> Assert.True(false, "Should never happend")
