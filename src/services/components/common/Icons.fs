namespace MyTeam.Views

open MyTeam.Models.Enums
open Giraffe.GiraffeViewEngine
open MyTeam.Shared.Components

[<AutoOpen>]
module IconComponents =

    let fromReactComponent c = 
        rawText <| Fable.Helpers.ReactServer.renderToString(c)

    let (!!) c = fromReactComponent c

    let fa name = 
        sprintf "fa fa-%s" name

    let icon name title =
        !!(Icons.icon name title)


    let playerStatusIcon (status: PlayerStatus) =                   
        !!(match status with
            | PlayerStatus.Aktiv -> Icons.playerStatusIcon MyTeam.Shared.Domain.PlayerStatus.Aktiv
            | PlayerStatus.Veteran -> Icons.playerStatusIcon MyTeam.Shared.Domain.PlayerStatus.Veteran
            | PlayerStatus.Trener -> Icons.playerStatusIcon MyTeam.Shared.Domain.PlayerStatus.Trener
            | _ -> Icons.playerStatusIcon MyTeam.Shared.Domain.PlayerStatus.Inaktiv)       

    let eventIcon (status: EventType) size =                   
        !!(match status with
            | EventType.Kamp -> Icons.eventIcon MyTeam.Shared.Domain.EventType.Kamp size
            | EventType.Trening -> Icons.eventIcon MyTeam.Shared.Domain.EventType.Trening size
            | EventType.Diverse -> Icons.eventIcon MyTeam.Shared.Domain.EventType.Diverse size
            | _ -> Icons.eventIcon MyTeam.Shared.Domain.EventType.Alle size
            
            )                             
            