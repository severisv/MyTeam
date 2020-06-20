module MyTeam.Games.Pages.Add

open Giraffe.GiraffeViewEngine
open MyTeam
open Shared
open Shared.Domain
open MyTeam.Games
open MyTeam.Views
open MyTeam.Views.BaseComponents
open Shared.Components
open Shared.Components.Nav
open System
open Shared.Domain.Members
open Fable.React.Props
open MyTeam
open System.Linq


let enumToList<'a> = (Enum.GetValues(typeof<'a>) :?> ('a [])) |> Array.toList


let view (club: Club) user (ctx: HttpContext) =

    let db = ctx.Database         

    [
        mtMain [_class "mt-main--narrow"] [
            block [] [
                Client.view2
                    "add-game"
                    Client.Features.Games.Add.element 
                    {Teams = club.Teams; GameTypes = Enums.getValues<GameType>()}
            
            ]
        ]
        !!Client.Features.Common.Admin.coachMenu
    ]
    |> layout club user (fun o -> { o with Title = "Ny kamp" }) ctx
    |> OkResult

