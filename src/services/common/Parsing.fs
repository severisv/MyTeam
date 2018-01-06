namespace Services

open System


[<AutoOpen>]
module Parsing = 
    let parseGuid (str: string) = System.Guid(str)
