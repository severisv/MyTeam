namespace MyTeam

open Giraffe
open Giraffe.GiraffeViewEngine
open Giraffe.GiraffeViewEngine.Attributes

[<AutoOpen>]
module Pages = 

    let layout (content: XmlNode list) =
        html [] [
            head [] [
                title [] [(encodedText "Giraffe")]
            ]
            body [] content
        ]
