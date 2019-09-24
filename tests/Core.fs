module NomFs.Tests.Core

open System
open Xunit

open NomFs.Core
open NomFs.Result
open NomFs.Bytes.Complete
open NomFs.Combinator

let extractOk res =
    match res with
    | Ok (input, res) -> (input, res)
    | e -> failwithf "Should never happen: %A" e

let extractErr res =
    match res with
    | Error (Err e) -> e
    | e -> failwithf "Should never happen: %A" e


type Color = {
    Red: byte;
    Green: byte;
    Blue: byte;
}

[<Fact>]
let ``color test`` () =

    let fromHex i =
        try
            let i = i |> Seq.toArray |> String
            Ok (Convert.ToByte(i, 16))
        with
            | e -> Error e

    let isHexDigit (c: char) =
        "abcdefABCDEF1234567890"
        |> Seq.contains c
   
    let hexPrimary =
        mapRes (takeWhileMN 2 2 isHexDigit) fromHex

    let hexColor input = result {
        let! input, _ = tag "#" input
        let! input, red = hexPrimary input
        let! input, green = hexPrimary input
        let! input, blue = hexPrimary input

        return! Ok (input, { Red = red; Green = green; Blue = blue; })}

    let c = hexColor "#2F14DF"

    match c with
    | Ok (input, co) ->
        Assert.Equal("", input)
        Assert.Equal(47uy, co.Red)
        Assert.Equal(20uy, co.Green)
        Assert.Equal(223uy, co.Blue)
    | e ->
        Assert.False(true, (sprintf "Should never happend: %A" e))
