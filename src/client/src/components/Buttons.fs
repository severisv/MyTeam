namespace Shared.Components

open Fable.React
open Fable.React.Props
open Shared
open Shared.Util

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

    
    let btnAnchor href (buttonType : ButtonType) (size : ButtonSize) attributes children =
        a (Html.mergeClasses 
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

        let props = props
                    |> List.filter (fun p -> not (p :? ButtonSize || p :? ButtonType))
       
        button 
            (Html.mergeClasses 
                 [ Class 
                   <| sprintf "btn btn-%s btn-%s" (Strings.toLower buttonType) 
                          (Strings.toLower size) ] props) children                  
