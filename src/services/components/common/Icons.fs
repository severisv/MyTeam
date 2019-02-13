namespace MyTeam.Views

open MyTeam.Models.Enums
open Giraffe.GiraffeViewEngine
open Shared.Components
open Shared


[<AutoOpen>]
module IconComponents =

    let fromReactComponent c = 
        rawText <| Fable.Helpers.ReactServer.renderToString(c)

    let (!!) c = fromReactComponent c

    let fa name = 
        sprintf "fa fa-%s" name

    let icon name title =
        !!(Icons.icon name title)


    let playerStatusIcon (status: Domain.PlayerStatus) =                   
        !!(match status with
            | Domain.PlayerStatus.Aktiv -> Icons.playerStatusIcon Domain.PlayerStatus.Aktiv
            | Domain.PlayerStatus.Veteran -> Icons.playerStatusIcon Domain.PlayerStatus.Veteran
            | Domain.PlayerStatus.Trener -> Icons.playerStatusIcon Domain.PlayerStatus.Trener
            | _ -> Icons.playerStatusIcon Domain.PlayerStatus.Inaktiv)       

    let eventIcon (status: EventType) size =                   
        !!(match status with
            | EventType.Kamp -> Icons.eventIcon Domain.EventType.Kamp size
            | EventType.Trening -> Icons.eventIcon Domain.EventType.Trening size
            | EventType.Diverse -> Icons.eventIcon Domain.EventType.Diverse size
            | _ -> Icons.eventIcon Domain.EventType.Alle size
            
            )                             
            