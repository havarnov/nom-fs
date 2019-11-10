module NomFs.Tests.Json

open System

open Xunit

open NomFs.Core

open NomFs.Examples.Json


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
    match parser j with
    | Ok (_, Array a) ->
        Assert.Equal(3, Seq.length a)
        let s = Seq.filter (fun i -> match i with | Str i -> i = "foo" | _ -> false) a
        let _ = Assert.Single(s)
        let b = Seq.filter (fun i -> match i with | Boolean b -> not b | _ -> false) a
        let _ = Assert.Single(b)
        let o = Seq.filter (fun i -> match i with | Object _ -> true | _ -> false) a
        let o = match Assert.Single(o) with | Object o -> o | _ -> raise (Exception "should never happen")
        Assert.Equal(JsonValue.Boolean true, o.Item "ball")
        Assert.Equal(Str "hav", o.Item "snall")
        Assert.Equal(Num 1234.123e-12, o.Item "ja")
        Assert.Equal(Num 1234.123e+12, o.Item "nei")
    | _ -> Assert.True(false, "Should never happend")
