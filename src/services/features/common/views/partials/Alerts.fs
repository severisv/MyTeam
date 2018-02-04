namespace MyTeam.Views

open Giraffe.GiraffeViewEngine
open MyTeam

[<AutoOpen>]
module Alerts =

    type AlertType = 
        | Success
        | Info
        | InfoSubtle
        | Danger
        | Warning
    
    let alertId alertType =     
        Enums.toString alertType 
        |> toLower

    let alertClass alertType =     
        Enums.toString alertType 
        |> toLower
        |> sprintf "alert alert-%s"

    let alertIcon alertType =     
        match alertType with
        | Success -> icon <| fa "check"
        | Info -> icon <| fa "info"
        | InfoSubtle -> icon <| fa "info"
        | Danger -> icon <| fa "warning"
        | Warning -> icon <| fa "warning"

    let alert alertType text = 
        div [_class <| alertClass alertType; _id <| alertId alertType] [
            alertIcon alertType
            whitespace
            span [_class "alert-content"] [ encodedText text ]
            button [_class "close"; _type "button";attr "data-dismiss" "alert"; attr "aria-hidden" "true" ] [ encodedText "×" ]
        ]

    let alerts = emptyText
                // @if (!Context.Request.IsAjaxRequest())
                // {
                //         <div mt-alert="Success">@ViewBag.AlertSuccess</div>
                //         <div mt-alert="Info">@ViewBag.AlertInfo</div>
                //         <div mt-alert="InfoSubtle">@ViewBag.AlertInfoSubtle</div>
                //         <div mt-alert="Warning">@ViewBag.AlertWarning</div>
                //         <div mt-alert="Danger">@ViewBag.AlertDanger</div>
                // }
                // else
                // {
                //     <script>
                //         window.mt.clearAlerts();
                //         @foreach(var value in Enum.GetValues(typeof(AlertType)))
                //         {
                //             var alert = ViewData[$"AjaxAlert{value}"] as string;
                //             if (alert != null)
                //             {
                //                 var alertType = value.ToString().ToLower();
                //                 <text>window.mt.alert('@alertType', '@alert');</text>
                //             }
                //         }
                //     </script>
                // }
