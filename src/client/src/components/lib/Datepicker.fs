module Shared.Components.Datepicker

open Fable.Core.JsInterop
open Fable
open Fable.Core
open Fable.Import
open Fable.React
open Fable.React.Props
open Shared.Util
open Shared.Util.ReactHelpers
open Shared

type DatePickerState = { Value: System.DateTime option }

type DateChangeHandler =
    | OnDateChange of (System.DateTime option -> unit)
    interface IHTMLProp



#if FABLE_COMPILER
let DatePicker: React.ReactElementType<obj> = importDefault "react-datepicker"

let setDefaultLocale: string -> unit = import "setDefaultLocale" "react-datepicker"

let no: obj = importDefault "date-fns/locale/nb"
#else
let DatePicker: React.ReactElementType<obj> =
    HtmlTag "input" :> ReactElementType<obj>

let no: obj = null
#endif

let datePicker (props: IHTMLProp list) =

    let value =
        props
        |> List.map (function
            | :? HTMLAttr as attr ->
                match attr with
                | Value v -> Date.tryParse <| string v
                | _ -> None
            | _ -> None)
        |> List.choose id
        |> List.tryHead

    let handleChange =
        props
        |> List.tryFind (fun p -> p :? DateChangeHandler)
        |> Option.map (fun p ->
            let fn = p :?> DateChangeHandler
            let (OnDateChange handleChange) = fn
            handleChange)
        |> Option.defaultValue ignore

    komponent<IHTMLProp list, DatePickerState> props { Value = value } None (fun (props, state, setState) ->
#if FABLE_COMPILER
        ReactElementType.create
            DatePicker
            (props
             |> Html.mergeClasses [
                 Type "text"
                 HTMLAttr.Custom("placeholder", "06.06.1987")
                 HTMLAttr.Custom("selected", state.Value)
                 HTMLAttr.Custom("locale", no)
                 HTMLAttr.Custom("dateFormat", "dd.MM.yyyy")
                 OnChange (fun e ->
                     let date =
                         Date.tryParse <| string e
                         |> Option.map (fun d -> d.Date.ToUniversalTime())

                     printf "%O" date

                     handleChange date
                     setState (fun state props -> { state with Value = date }))
                ]
             |> keyValueList CaseRules.LowerFirst)
            []
#else
        input [
            Class "form-control"
            Type "text"
            Placeholder "06.06.1987"
            HTMLAttr.Custom("selected", state.Value)
            HTMLAttr.Custom("dateFormat", "dd.MM.yyyy")
        ]
#endif
    )
