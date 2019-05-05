module Shared.Components.Datepicker

open Fable.Core.JsInterop
open Fable.Core
open Fable.Import
open Fable.Helpers.React.Props
open Shared.Util
   
let DatePicker : React.ComponentClass<obj> = importDefault "react-datepicker"

let datePicker (props : IHTMLProp list) =    

    Fable.Helpers.React.from DatePicker
                             (props
                             |> Html.mergeClasses
                                           [Type "text"
                                            OnChange(fun e -> printf "%O" e )]
                             |> keyValueList CaseRules.LowerFirst) []