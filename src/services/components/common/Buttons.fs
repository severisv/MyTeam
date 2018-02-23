namespace MyTeam.Views

open MyTeam
open Giraffe.GiraffeViewEngine

[<AutoOpen>]
module Buttons =  
        
    type ButtonType = 
        | Primary

    type ButtonSize = 
        | Sm


    let buttonLink href (buttonType: ButtonType) (size: ButtonSize) attributes children  =
        a 
            ([_href href;
              _class <| sprintf "btn btn-%s btn-%s" 
                                        (Enums.toLowerString buttonType) 
                                        (Enums.toLowerString size)
             ] 
             |> mergeAttributes attributes) 
            children
                               