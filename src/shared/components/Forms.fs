module MyTeam.Shared.Components.Forms

open Fable.Core
open Fable.Core.JsInterop
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Import.React
open MyTeam.Shared.Util.Html

let textInput attr =
    input (attr
           |> mergeClasses [ Class "form-control"
                             Type "text" ])

type FormProps =
    | Horizontal of int
    interface IHTMLProp

type IFormAttributes =
    inherit HTMLAttributes
    abstract horizontal : int option with get, set

let formRow props lbl input =
    let p : IFormAttributes = keyValueList CaseRules.LowerFirst props |> unbox
    div (props |> mergeClasses [ Class "form-group" ]) [ label 
                                                             [ Class 
                                                               <| sprintf "control-label %s" 
                                                                      (p.horizontal
                                                                       |> function 
                                                                       | Some h -> 
                                                                           sprintf "col-sm-%i" h
                                                                       | None -> "") ] lbl
                                                         div [ Class(p.horizontal
                                                                     |> function 
                                                                     | Some h -> 
                                                                         sprintf "col-sm-%i" 
                                                                             (12 - h)
                                                                     | None -> "") ] input ]

let form (props: IHTMLProp list) children =
    let p : IFormAttributes = keyValueList CaseRules.LowerFirst props |> unbox
    form ((props @ [ OnSubmit(fun e -> e.preventDefault()) ])
          |> mergeClasses [ Class(match p.horizontal with
                                  | (Some _) -> "form-horizontal"
                                  | _ -> "") ]) children
