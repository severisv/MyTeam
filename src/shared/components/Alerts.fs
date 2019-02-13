module Shared.Components.Alerts

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Shared
open Shared.Components
open Shared.Components.Base

type AlertType =
    | Success
    | Info
    | InfoSubtle
    | Danger
    | Warning

let internal alertId alertType = alertType |> Strings.toLower

let internal alertClass alertType =
    alertType
    |> Strings.toLower
    |> sprintf "alert alert-%s"

let internal alertIcon =
    function 
    | Success -> Icons.check
    | Info -> Icons.info ""
    | InfoSubtle -> Icons.info ""
    | Danger -> Icons.warning
    | Warning -> Icons.warning

let internal alert alertType text =
    div [ Class <| alertClass alertType
          Id <| alertId alertType ] 
        [ alertIcon alertType
          whitespace
          span [ Class "alert-content" ] [ str text ]
          button [ Class "close"
                   Type "button"
                   HTMLAttr.Custom("data-dismiss", "alert")
                   HTMLAttr.Custom("aria-hidden", "true") ] [ str "×" ] ]

let info = alert Info
let infoSubtle = alert InfoSubtle
let danger = alert Danger
let warning = alert Warning
let success = alert Success
