module NomFs.Mqtt.Decoder

type Dummy = unit

(*
open System
open NomFs.Bytes.Streaming
open NomFs.Combinator
open NomFs.Core
open NomFs.Result

type MqttParserError<'I> =
    | MalformedPacket of string
    | Nom of ('I * NomFs.Core.ErrorKind)

type MqttParserResult<'I, 'O> = NomFs.Core.ParseResult<'I, 'O>

type MqttHeader = {
    PacketType: uint8
    Flags: uint8
    PacketSize: uint32
}

(*
fn parse_first_byte(input: (&[u8], usize)) -> IResult<(&[u8], usize), (u8, u8)> {
    let (rest, fst) = bit_take::<_, _, _, Error<_>>(4usize)(input)?;
    let (rest, snd) = bit_take::<_, _, _, Error<_>>(4usize)(rest)?;
    Ok((rest, (fst, snd)))
}
*)
let parse_first_byte (input: ReadOnlyMemory<uint8>) : MqttParserResult<ReadOnlyMemory<uint8>, uint8 * uint8> =
    failwith "not implemented"

(*

fn parse_variable_u32(input: &[u8]) -> MqttParserResult<&[u8], u32> {
    let mut rest = input;

    let mut result = 0u32;

    let mut shift = 0;
    loop {
        // https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html#_Toc3901011
        // The maximum number of bytes in the Variable Byte Integer field is four.
        if shift >= 4 {
            return Err(nom::Err::Failure(MalformedPacket(
                "The maximum number of bytes in the Variable Byte Integer field is four."
                    .to_string(),
            )));
        }

        let (rest_next, size) = take_first(rest)?;
        rest = rest_next;

        if size >= 128u8 {
            result += (size as u32 - 128u32) << (shift * 7);
            shift += 1;
        } else {
            result += (size as u32) << (shift * 7);
            break Ok((rest, result));
        }
    }
}

fn take_first<Input>(input: Input) -> MqttParserResult<Input, Input::Item>
where
    Input: InputTake + InputLength + InputTakeFirst,
{
    map(take(1usize), |i: Input| i.take_first())(input)
}
*)

let take_first (input: ReadOnlyMemory<uint8>) :Result<ReadOnlyMemory<uint8> * uint8, MqttParserError<ReadOnlyMemory<uint8>>> =
    map (take 1) (fun i -> i.Span.Item 0)(input)

let parse_variable_u32 (input: ReadOnlyMemory<uint8>) : Result<ReadOnlyMemory<uint8> * uint32, MqttParserError<ReadOnlyMemory<uint8>>> = result {
    let mutable rest = input
    let mutable result = 0u
    let mutable shift = 0u
    let mutable response = None
    while response.IsNone do
        if shift >= 4u then
            response <- Some (Error (Err.Failure (rest, ErrorKind.Many)))
        else

        let! rest, size = take_first(rest)
         (*
        let! rest, size =
            match take_first(rest) with
            | Ok (rest_next, size) -> Ok (rest_next, size)
            | MqttParserResult.Error e ->
                Error ()
         *)

        if size >= 128uy then
            result <- result + ((uint32 size) - 128u) <<< ((int32 shift) * 7)
            shift <- shift + 1u
        else
            result <- result + (uint32 size) <<< ((int32 shift) * 7)
            response <- Some (Ok (rest, result))

    return! response.Value
}*)
