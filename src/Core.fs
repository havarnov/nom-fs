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
    | MapRes
    | TakeWhileMN
    | Tuple
    | Eof

type IResult<'I, 'O> = Result<'I * 'O, Err<'I * ErrorKind>>
