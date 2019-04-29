module Shared.Components.Forms

open Shared.Components.Base
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Shared.Util.Html
open Client.Util.ReactHelpers

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

type InputState = {
    IsTouched: bool
}

type InputProps =
    | Validation of (bool * string)
    interface IHTMLProp

let textInput (attr: IHTMLProp list) =
    komponent<IHTMLProp list, InputState>
         attr
         { IsTouched = false }
         None
         (fun (props, state, setState) ->

            let props =
                attr
                |> List.tryFind (fun p -> p :? InputProps)
                |> Option.map (fun p -> p :?> InputProps)
                
            let (isValid, validationMessage) =
                props
                |> function
                | Some(Validation (i, m)) when state.IsTouched -> i, Some m
                | _ -> true, None
            
            fragment [] [
                 input (attr
                   |> mergeClasses [OnBlur (fun _ -> setState (fun state props -> { state with IsTouched = true }))
                                    Class <| sprintf "form-control input-isValid--%b" isValid
                                    Type "text" ])
                 not isValid &?                 
                     (validationMessage =>
                        fun validationMessage ->
                            span [Class "input-validation-message"] [str validationMessage])
            ]
        )
   

let dateInput attr =
    input (attr
           |> mergeClasses [Class "form-control"
                            Type "text" ])    
    
type SelectOption<'a> = { Name: string; Value: 'a }
let selectInput attr options =
    select (attr
           |> mergeClasses [Class "form-control"])
           (options |> List.map(fun o ->
                                    option [Value o.Value] [str o.Name]
        ))
    

let validationMessage messages =
    span [Class "text-danger"] (messages |> List.map str)
