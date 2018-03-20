namespace MyTeam

[<AutoOpen>]
module Option =

   let toOption (n : System.Nullable<_>) = 
       if n.HasValue 
           then Some n.Value 
           else None
