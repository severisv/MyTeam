module Shared.Components.Forms

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Shared.Util.Html

type FormProps =
    | Horizontal of int
    interface IHTMLProp

let form (props : IHTMLProp list) children =
    let horizontal =
        props
        |> List.tryFind (fun p -> p :? FormProps)
        |> Option.map (fun p -> p :?> FormProps)
    form ((props @ [ OnSubmit(fun e -> e.preventDefault()) ])
          |> mergeClasses [ Class(match horizontal with
                                  | Some(Horizontal h) -> "form-horizontal"
                                  | _ -> "") ]) children

let formRow (props : IHTMLProp list) lbl input =
    let horizontal =
        props
        |> List.tryFind (fun p -> p :? FormProps)
        |> Option.map (fun p -> p :?> FormProps)
    div (props |> mergeClasses [ Class "form-group" ]) 
                               [ label 
                                     [ Class 
                                       <| sprintf "control-label %s" 
                                              (horizontal
                                               |> function 
                                                | Some(Horizontal h) -> sprintf "col-sm-%i" h
                                                | None -> "") ] lbl
                                 div [ Class(horizontal
                                             |> function 
                                              | Some(Horizontal h) -> sprintf "col-sm-%i" (12 - h)
                                              | None -> "") ] input ]

let textInput attr =
    input (attr
           |> mergeClasses [ Class "form-control"
                             Type "text" ])

let validationMessage messages = span [ Class "text-danger" ] (messages |> List.map str)
