namespace MyTeam

open System

[<AutoOpen>]
module Option =
    let fromNullable (n : System.Nullable<_>) =
        if n.HasValue then Some n.Value
        else None
    
    let toNullable =
        function 
        | None -> Nullable()
        | Some x -> Nullable(x)
    
    let toString =
        function 
        | None -> ""
        | Some x -> x.ToString()
    
    let (|Null|Value|) (x: _ Nullable) =
        if x.HasValue then Value x.Value else Null