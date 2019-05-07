module Shared.Components.Datepicker

open Fable.Core.JsInterop
open Fable.Core
open Fable.Import
open Fable.Helpers.React.Props
open Shared.Util
open Shared.Util.ReactHelpers
open Shared
   
type DatePickerState = {
    Value: System.DateTime option
}   

type DateChangeHandler =
        | OnDateChange of (System.DateTime option -> unit)
        interface IHTMLProp
   
let DatePicker : React.ComponentClass<obj> = importDefault "react-datepicker"

let datePicker (props : IHTMLProp list) =
           
    let value = props
                |> List.map (
                    function
                    | :? HTMLAttr as attr ->
                            match attr with
                            | Value v -> Date.tryParse <| string v
                            | _ -> None
                    | _ -> None
                        )
                |> List.choose id
                |> List.tryHead
              
                
    let handleChange = props
                        |> List.tryFind (fun p -> p :? DateChangeHandler)
                        |> Option.map (fun p ->
                                       let fn = p :?> DateChangeHandler
                                       let (OnDateChange handleChange) = fn
                                       handleChange
                                       )
                        |> Option.defaultValue (fun _ -> ())                                              
    
    komponent<IHTMLProp list, DatePickerState>
        props
        { Value = value }
        None
        (fun (props, state, setState) ->                

            
            Fable.Helpers.React.from DatePicker
                                     (props
                                     |> Html.mergeClasses
                                                   [Type "text"
                                                    HTMLAttr.Custom ("selected", state.Value) 
                                                    OnChange(fun e ->
                                                        let date = Date.tryParse <| string e
                                                        handleChange date
                                                        setState (fun state props -> { state with Value = date }) )]
                                     |> keyValueList CaseRules.LowerFirst) [])