module Client.Features.Players.Stats



open Client.Components

open Fable.React
open Fable.React.Props
open Shared
open Shared.Components
open Shared.Components.Base
open Shared.Components.Datepicker
open Shared.Components.Forms
open Shared.Domain
open Shared.Util
open Shared.Util.ReactHelpers
open Shared.Validation
open System
open Thoth.Json
open Shared.Components.Links
open Client.Components
open Shared.Util.ReactHelpers
open Thoth.Json
open Fable.React
open Fable.React.Props
open Shared
open Shared.Components.Layout
open Shared.Components.Tabs
open Shared.Components
open Shared.Components.Icons
open Shared.Components.Base
open Shared.Components.Nav
open Shared.Domain.Members
open System
open Client.Events
open Client.Util


type Props = { PlayerId: Guid }


type TeamStats =
    { TeamId: Guid
      TeamName: string
      Games: int
      Goals: int
      Assists: int
      YellowCards: int
      RedCards: int }

type PlayerStats = { Year: int; Teams: TeamStats list }


let containerId = sprintf "async-load-player-stats"

let element =
    FunctionComponent.Of (fun (props: Props) ->


        let stats = Hooks.useState<PlayerStats list> ([])

        Hooks.useEffect (
            (fun _ ->
                Http.get
                    (sprintf "/api/stats/player/%O" props.PlayerId)
                    Decode.Auto.fromString<PlayerStats list>
                    { OnSuccess = stats.update
                      OnError = printf "%O" }),
            [| props.PlayerId |]
        )

        if stats.current
           |> List.filter (fun s -> not s.Teams.IsEmpty)
           |> List.isEmpty
           |> not then
            div
                [ Class "playerStats" ]
                ([ h4 [] [ str "Stats" ] ]
                 @ (stats.current
                    |> List.mapi (fun i year ->
                        let isCollapsed =
                            (i = 0 || (i = (stats.current.Length - 1)))
                            |> function
                                | true -> Open
                                | _ -> Collapsed


                        Collapsible.collapsible
                            isCollapsed
                            [ div [ Class "playerStats-year" ] [
                                  (if year.Year = 0 then
                                       "Totalt"
                                   else
                                       string year.Year)
                                  |> str
                              ] ]
                            [ table [ Class "table playerStats" ] [
                                  tbody
                                      []
                                      (year.Teams
                                       |> List.map (fun season ->
                                           tr [] [
                                               td [] [ str season.TeamName ]
                                               td [ Class "playerStats--value" ] [
                                                   Icons.player "Kamper"
                                                   str <| string season.Games
                                               ]
                                               td [ Class "playerStats--value" ] [
                                                   Icons.goal "Mål"
                                                   str <| string season.Goals
                                               ]
                                               td [ Class "playerStats--value" ] [
                                                   Icons.assist "Assists"
                                                   str <| string season.Assists
                                               ]
                                               td
                                                   [ Class "playerStats--value kort" ]
                                                   (if season.YellowCards > 0 then
                                                        [ Icons.yellowCard "Gule kort"
                                                          str <| string season.YellowCards ]
                                                    else
                                                        [])
                                               td
                                                   [ Class "playerStats--value kort" ]
                                                   (if season.RedCards > 0 then
                                                        [ Icons.redCard "Røde kort"
                                                          str <| string season.RedCards ]
                                                    else
                                                        [])
                                           ]))
                              ] ])))
        else
            fr [] [])

hydrate2 containerId Decode.Auto.fromString<Props> element
