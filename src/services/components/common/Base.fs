namespace MyTeam.Views

open Giraffe.GiraffeViewEngine
open MyTeam

[<AutoOpen>]
module BaseComponents =

    let whitespace = rawText "&nbsp;"

    let _empty = attr "" ""

    let antiforgeryToken ctx =
        let token = Antiforgery.getToken ctx
        input [_name "__RequestVerificationToken";_value token;_type "hidden" ]

    
    type HtmlValue = 
        | XmlNode of XmlNode 
        | Str of string
        | Number of int

    let toXmlNode = function
        | XmlNode n -> n
        | Str s -> encodedText s
        | Number i -> encodedText <| str i    