module NomFs.ReadOnlyMemory

open NomFs
open System

let inline sequenceEqual (a: ReadOnlyMemory<'a>) (b: ReadOnlyMemory<'a>) =
    if a.Length <> b.Length then
        false
    else
        let rec sequenceEqualInternal (aInternal: ReadOnlyMemory<'a>) (bInternal: ReadOnlyMemory<'a>) =
            if aInternal.Length = 0 then
                true
            elif aInternal.Span.[0] = bInternal.Span.[0] then
                sequenceEqualInternal (aInternal.Slice(1)) (bInternal.Slice(1))
            else
                false
        sequenceEqualInternal a b

let inline takeWhile (predicate: 'a -> bool) (source: ReadOnlyMemory<'a>) =
    let mutable splitAt = None
    let mutable current = 0
    while Option.isNone splitAt && current < source.Length do
        if not (predicate source.Span.[current])
        then
            splitAt <- Some current

        current <- current + 1

    match splitAt with
    | None -> source
    | Some splitAt ->
        source.Slice(0, splitAt)

let inline contains (value: 'a) (source: ReadOnlyMemory<'a>) =
    let mutable i = 0
    let mutable found = false
    while i < source.Length && not found do
        if source.Span.[i] = value
        then
            found <- true
        i <- i + 1
    found