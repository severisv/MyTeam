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
        interface IHTMLProp


    type ButtonSize =
        | Sm
        | Normal
        | Lg
        interface IHTMLProp

    
    let buttonLink href (buttonType : ButtonType) (size : ButtonSize) attributes children =
        a 
            (Html.mergeClasses 
                 [ Href href
                   
                   Class 
                   <| sprintf "btn btn-%s btn-%s" (Strings.toLower buttonType) 
                          (Strings.toLower size) ] attributes) children
    

    let btn (props : IHTMLProp list) children =
        let buttonType = props
                        |> List.tryFind (fun p -> p :? ButtonType)
                        |> Option.map (fun p -> p :?> ButtonType)
                        |> Option.defaultValue Default

        let size = props
                    |> List.tryFind (fun p -> p :? ButtonSize)
                    |> Option.map (fun p -> p :?> ButtonSize)
                    |> Option.defaultValue Normal        

        button 
            (Html.mergeClasses 
                 [ Class 
                   <| sprintf "btn btn-%s btn-%s" (Strings.toLower buttonType) 
                          (Strings.toLower size) ] props) children                  
