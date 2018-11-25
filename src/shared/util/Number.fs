namespace MyTeam

open System

module Number =
       
        let parse (s: string) = Int32.Parse s

        let tryParse (s: string) = 
            match Int32.TryParse s with
                | true, i -> Some i
                | _ -> None