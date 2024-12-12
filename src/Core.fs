module NomFs.Core

open System

type Needed =
    | Unknown
    | Size of uint32

type Err<'E> =
    | Incomplete of int
    | Err of 'E
    | Failure of 'E

type ErrorKind =
    | Tag
    | Digit
    | Alpha
    | Alphanumeric
    | MapRes
    | TakeWhileMN
    | Tuple
    | Eof
    | OneOf
    | Escape
    | SeparatedList
    | Many
    | Float

type ParseResult<'I, 'O> = Result<'I * 'O, Err<'I * ErrorKind>>

type Parser<'I, 'O> = 'I -> ParseResult<'I, 'O>

let inline m (str: string) = str.AsMemory ()

let inline a (i: 'a array) = ReadOnlyMemory i
