module Shared.Components.Forms

open Shared.Components.Base
open Shared.Components.Datepicker
open Fable.React
open Fable.React.Props
open Shared.Util.Html
open Shared.Util.ReactHelpers

type FormProps =
    | Horizontal of int
    interface IHTMLProp

let form (props : IHTMLProp list) children =
    let horizontal =
        props
        |> List.tryFind (fun p -> p :? FormProps)
        |> Option.map (fun p -> p :?> FormProps)
    form ((props @ [OnSubmit(fun e -> e.preventDefault()) ])
          |> mergeClasses [Class(match horizontal with
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
    | Validation of Result<unit, string> list
    interface IHTMLProp

type IsTouched =
    | IsTouched of bool
    interface IHTMLProp

let textInput (attr: IHTMLProp list) =
    komponent<IHTMLProp list, InputState>
         attr
         { IsTouched = false }
         None
         (fun (attr, state, setState) ->

            let isTouched =
                attr
                |> List.tryFind (fun p -> p :? IsTouched)
                |> Option.map (fun p -> p :?> IsTouched)       
                |> function
                | Some (IsTouched isTouched) -> isTouched
                | _ -> state.IsTouched
                
            let validation =
                attr
                |> List.tryFind (fun p -> p :? InputProps)
                |> Option.map (fun p -> p :?> InputProps)       
              

            let (isValid, validationMessage) =
                validation
                |> function
                | Some(Validation e) when isTouched
                     -> 
                        e
                        |> List.map (function | Ok () -> (true, None) | Error m -> false, Some m)
                        |> List.fold (fun acc (isValid, m) -> if not isValid then (isValid, m) else acc) (true, None)
                | _ -> true, None
            
            fragment [] [
                 input (attr
                   |> List.filter (fun p -> not <| p:? InputProps)
                   |> List.filter (fun p -> not <| p:? IsTouched)
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
    datePicker
            (attr
           |> mergeClasses [Class "form-control"])    
    
type SelectOption<'a> = { Name: string; Value: 'a }
let selectInput attr options =
    select (attr
           |> mergeClasses [Class "form-control"])
           (options |> List.map(fun o ->
                                    option [Value o.Value] [str o.Name]
        ))
    
let checkboxInput attr lbl value onChange  =
    label (attr |> mergeClasses [Class "control-label input-form-checkbox"])           
          (lbl @ [
            input [ Class "form-control"
                    Type "checkbox"
                    Checked value
                    OnChange(fun input -> onChange input.Checked) ]]
        )   
               
        
    
    

let validationMessage messages =
    span [Class "text-danger"] (messages |> List.map str)
