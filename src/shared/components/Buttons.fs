namespace MyTeam.Components
open Fable.Helpers.React
open Fable.Helpers.React.Props
open MyTeam
open MyTeam.Shared.Util

[<AutoOpen>]
module Buttons =  
        
    type ButtonType = 
        | Primary
        | Success
        | Danger
        | Default

    type ButtonSize = 
        | Sm
        | Normal
        | Lg

    let buttonLink href (buttonType: ButtonType) (size: ButtonSize) attributes children  =
        a 
            (Html.mergeClasses [Href href;
              Class <| sprintf "btn btn-%s btn-%s" 
                                        (Strings.toLower buttonType) 
                                        (Strings.toLower size)
             ] attributes) 
            children
    
    let btn (buttonType: ButtonType) (size: ButtonSize) attributes children =
        button (Html.mergeClasses [Class <| sprintf "btn btn-%s btn-%s" 
                                            (Strings.toLower buttonType) 
                                            (Strings.toLower size)]
                               attributes) children                               