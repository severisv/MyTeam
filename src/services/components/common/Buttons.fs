namespace MyTeam.Views

open MyTeam
open Giraffe.GiraffeViewEngine

[<AutoOpen>]
module Buttons =  
        
    let str e = Enums.toString e |> toLower

    type ButtonType = 
        | Primary

    type ButtonSize = 
        | Sm


    let buttonLink href buttonType size attributes children  =
        a 
            ([_href href;_class <| sprintf "btn btn-%s btn-%s" (str buttonType) (str size)] |> mergeAttributes attributes) 
            children
                               