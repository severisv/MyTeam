namespace MyTeam.Views

open Giraffe.GiraffeViewEngine
open MyTeam

[<AutoOpen>]
module BaseComponents =

    let empty = rawText ""
    let whitespace = rawText "&nbsp;"

    let _empty = attr "" ""

    let antiforgeryToken ctx =
        let token = Antiforgery.getToken ctx
        input [_name "__RequestVerificationToken";_value token;_type "hidden" ]

    let number = string >> encodedText
    let (=>) optn fn =
        optn
        |> Option.map fn
        |> Option.defaultValue empty    
        
