module NomFs.Core

open System
open NomFs.Result

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

type IResult<'I, 'O> = Result<'I * 'O, Err<'I * ErrorKind>>

let inline m (str: string) = str.AsMemory()

let inline a (i: 'a array) = ReadOnlyMemory(i)
