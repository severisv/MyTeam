namespace MyTeam

open System

[<AutoOpen>]
module Option =

    let toOption (n : System.Nullable<_>) = 
       if n.HasValue 
           then Some n.Value 
           else None

    let toNullable =
        function
        | None -> Nullable()
        | Some x -> Nullable(x)
