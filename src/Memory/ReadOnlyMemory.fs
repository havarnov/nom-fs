module NomFs.Memory.ReadOnlyMemory

open System

let takeWhile (predicate: 'a -> bool) (source: ReadOnlyMemory<'a>) =
    let mutable splitAt = None
    let mutable current = 0
    while Option.isNone splitAt do
        if not (predicate source.Span.[current])
        then
            splitAt <- Some current
        
        current <- current + 1
    
    match splitAt with
    | None -> source
    | Some splitAt ->
        source.Slice(0, splitAt)

let contains (value: 'a) (source: ReadOnlyMemory<'a>) =
    let mutable i = 0
    let mutable found = false
    while i < source.Length && not found do
        if source.Span.[i] = value
        then
            found <- true
        i <- i + 1
    found