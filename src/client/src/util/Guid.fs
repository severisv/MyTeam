module Guid

open System

let tryParse (s: string) =
    match Guid.TryParse s with
    | true, i -> Some i
    | _ -> None
