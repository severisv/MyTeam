module Shared.Components.Forms

open Shared.Components.Base
open Shared.Components.Datepicker
open Fable.React
open Fable.React.Props
open Fable.Import
open Fable.Core
open Fable.Core.JsInterop
open Shared.Util.Html
open Shared.Util.ReactHelpers
open Shared

let (|IsHTMLAttr|_|) (attr: IHTMLProp) =
    match attr with
    | :? HTMLAttr as a -> Some a
    | _ -> None


type FormProps =
    | Horizontal of int
    interface IHTMLProp

let form (props: IHTMLProp list) children =
    let horizontal =
        props
        |> List.tryFind (fun p -> p :? FormProps)
        |> Option.map (fun p -> p :?> FormProps)

    form
        ((props @ [ OnSubmit(fun e -> e.preventDefault ()) ])
         |> mergeClasses
             [ Class
                 (match horizontal with
                  | Some (Horizontal h) -> "form-horizontal"
                  | _ -> "") ])
        children

let formRow (props: IHTMLProp list) lbl input =
    let horizontal =
        props
        |> List.tryFind (fun p -> p :? FormProps)
        |> Option.map (fun p -> p :?> FormProps)

    div
        (props |> mergeClasses [ Class "form-group" ])
        [ label
            [ Class
              <| sprintf
                  "control-label %s"
                     (horizontal
                      |> function
                      | Some (Horizontal h) -> sprintf "col-sm-%i" h
                      | None -> "") ]
              lbl
          div
              [ Class
                  (horizontal
                   |> function
                   | Some (Horizontal h) -> sprintf "col-sm-%i" (12 - h)
                   | None -> "") ]
              input ]

type InputState = { IsTouched: bool }

type InputProps =
    | Validation of Result<unit, string> list
    interface IHTMLProp

type IsTouchedProps =
    | IsTouched of bool
    interface IHTMLProp

let validationMessage2 (isValid, message) =
    not isValid
    &? (message
        => fun message ->
            span [ Class "input-validation-message" ] [
                str message
            ])

let (|IsInputPropsAttr|_|) (attr: IHTMLProp) =
    match attr with
    | :? InputProps as a -> Some a
    | _ -> None

let (|IsIsTouchedProps|_|) (attr: IHTMLProp) =
    match attr with
    | :? IsTouchedProps as a -> Some a
    | _ -> None


let internal distillValidation validation isTouched =
    match (validation, isTouched) with
    | (Some e, true) when isTouched ->
        e
        |> List.map (function
            | Ok () -> (true, None)
            | Error m -> false, Some m)
        |> List.fold (fun acc (isValid, m) -> if not isValid then (isValid, m) else acc) (true, None)
    | _ -> true, None

let textInput (attr: IHTMLProp list) =
    komponent<IHTMLProp list, InputState> attr { IsTouched = false } None (fun (attr, state, setState) ->

        let isTouched =
            attr
            |> List.tryPick (function
                | IsIsTouchedProps (IsTouched v) -> Some v
                | _ -> Some state.IsTouched)
            |> Option.defaultValue state.IsTouched

        let validation =
            attr
            |> List.tryPick (function
                | IsInputPropsAttr (Validation v) -> Some v
                | _ -> None)



        let (isValid, vMessage) = distillValidation validation isTouched

        fragment [] [
            input
                (attr
                 |> List.filter (fun p -> not <| p :? InputProps)
                 |> mergeClasses [ OnBlur(fun _ -> setState (fun state props -> { state with IsTouched = true }))
                                   Class
                                   <| sprintf "form-control input-isValid--%b" isValid
                                   Type "text" ])
            validationMessage2 (isValid, vMessage)
        ])


let dateInput =

    FunctionComponent.Of(fun (attr: IHTMLProp list) ->


        let isTouchedState = Hooks.useState false

        let isTouched =
            attr
            |> List.tryPick (function
                | IsIsTouchedProps (IsTouched v) -> Some v
                | _ -> Some isTouchedState.current)
            |> Option.defaultValue isTouchedState.current

        let validation =
            attr
            |> List.tryPick (function
                | IsInputPropsAttr (Validation v) -> Some v
                | _ -> None)


        let (isValid, vMessage) = distillValidation validation isTouched
        fragment [] [
            datePicker
                (attr
                 |> mergeClasses [ Class "form-control"
                                   OnBlur(fun _ -> isTouchedState.update (fun _ -> true)) ])
            validationMessage2 (isValid, vMessage)
        ])

type SelectOption<'a> = { Name: string; Value: 'a }

let selectInput (attr: IHTMLProp list) options =
    let selectedValue =
        attr
        |> List.tryPick (function
            | IsHTMLAttr (Value v) -> Some v
            | _ -> None)

    select
        (attr |> mergeClasses [ Class "form-control" ])
        (options
         |> List.map (fun o ->
             option [ Value o.Value
                      Selected(selectedValue = Some o.Value) ] [
                 str o.Name
             ]))


[<Emit("Array.from($0)")>]
let internal asArray (a: obj): 'a array = failwith "JS Only"

type MultiSelectProps<'a> =
    { Options: SelectOption<'a> list
      Values: 'a list
      OnChange: 'a list -> unit }

let multiSelect<'a when 'a: equality> (props: MultiSelectProps<'a>) =
    select
        (mergeClasses [ Class "form-control"
                        OnChange(fun e ->
                            let options = e.target?selectedOptions |> asArray
                            props.OnChange
                                (props.Options
                                 |> List.map (fun o -> o.Value)
                                 |> List.filter (fun value ->
                                     options
                                     |> Array.exists (fun (o: Browser.Types.HTMLOptionElement) ->
                                         o.value = value.ToString())))


                            ) ] [
            Multiple true
         ])
        (props.Options
         |> List.map (fun o ->
             option [ Value o.Value
                      Selected(props.Values |> List.contains o.Value) ] [
                 str o.Name
             ]))

let checkboxInput attr lbl value onChange =
    label
        (attr
         |> mergeClasses [ Class "control-label input-form-checkbox" ])
        ([ input [ Class "form-control"
                   Type "checkbox"
                   Checked value
                   OnChange(fun input -> onChange input.Checked) ] ]
         @ lbl)


let validationMessage messages =
    span [ Class "text-danger" ] (messages |> List.map str)
