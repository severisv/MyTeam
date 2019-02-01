namespace MyTeam

open System

[<AutoOpen>]
module Helpers =
    
    let isNotNull = fun v -> v  |> (isNull >> not)
    
    let equals s1 s2 =
        s1 = s2

    let (=??) (first: string) (second: string) =
        if not <| String.IsNullOrWhiteSpace(first) then first else second          


    let (=?) (condition: bool) (first, second) =
        if condition then first else second       

    let never = fun _ -> false    