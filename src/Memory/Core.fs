module NomFs.Memory.Core

open System

let m (str: string) = str.AsMemory()

let a (i: 'a array) = i.AsMemory()
