module MyTeam.Games.Pages.List

open Giraffe.ViewEngine
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

let view (club: Club) (user: User option) selectedTeamShortName selectedYear (ctx: HttpContext) =

    let db = ctx.Database

    match selectedTeamShortName with
    | None -> club.Teams |> List.tryHead
    | Some s ->
        club.Teams
        |> List.tryFind (fun t -> t.ShortName |> toLower = (s |> toLower))
    |> function
        | None -> NotFound
        | Some selectedTeam ->

            let listGamesUrl = sprintf "/kamper/%s/%i"


            let years = Queries.listGameYears db selectedTeam.Id

            let selectedYear =
                selectedYear
                |> Option.defaultValue (
                    years
                    |> List.tryHead
                    |> Option.defaultValue DateTime.Now.Year
                )


            let isSelected =
                equals
                <| listGamesUrl selectedTeam.ShortName selectedYear

            let games =
                Queries.listGames db selectedTeam.Id selectedYear

            [ mtMain [] [
                block [] [
                    !!(Tabs.tabs
                        [ Class "team-nav" ]
                        (club.Teams
                         |> List.map
                             (fun team ->
                                 { Text = team.Name
                                   ShortText = team.ShortName
                                   Icon = Some(Icons.team "")
                                   Url = listGamesUrl team.ShortName selectedYear }))
                        isSelected)

                    !!(navListMobile (
                        { Items =
                              years
                              |> List.map
                                  (fun year ->
                                      { Text = string year
                                        Url = listGamesUrl selectedTeam.ShortName year })
                          Footer = None
                          IsSelected = isSelected }
                    ))
                    hr []

                    (if games.Length < 1 then
                         (!!(Alerts.info "Det er ikke lagt inn noen kamper for denne sesongen"))
                     else
                         div
                             [ _class "table gamesTable table--striped" ]
                             (games
                              |> List.map
                                  (fun game ->
                                      a [ _class "games-showGameLink"
                                          _href <| sprintf "/kamper/%O" game.Id ] [
                                          span [ _class "hidden-xs gameType" ] [
                                              !!(Icons.gameType game.Type)
                                          ]
                                          span [ _class "gamesTable-date" ] [
                                              !!(Icons.calendar "")
                                              |> withClass "text-subtle hidden-xxs"
                                              whitespace
                                              Date.formatShort >> encodedText <| game.DateTime
                                          ]
                                          span [ _class "gamesTable-team hidden-xs gamesTable-team--left" ] [
                                              encodedText game.HomeTeam
                                          ]
                                          span [ _class "gamesTable-team gamesTable-visible-xs gamesTable-team--left" ] [
                                              truncate 12 >> encodedText <| game.HomeTeam
                                          ]
                                          span [ _class "gamesTable-team" ] [
                                              span [ _class
                                                     <| sprintf
                                                         "gameResult gameResult--%s"
                                                         (game.Outcome
                                                          |> Option.map string
                                                          |> Option.defaultValue "") ] [
                                                  encodedText
                                                  <| sprintf
                                                      "%s - %s"
                                                      (game.HomeScore
                                                       |> Option.map string
                                                       |> Option.defaultValue "")
                                                      (game.AwayScore
                                                       |> Option.map string
                                                       |> Option.defaultValue "")
                                              ]
                                          ]
                                          span [ _class "gamesTable-team hidden-xs gamesTable-team--right" ] [
                                              encodedText game.AwayTeam
                                          ]
                                          span [ _class "gamesTable-team gamesTable-visible-xs gamesTable-team--right" ] [
                                              truncate 12 >> encodedText <| game.AwayTeam
                                          ]
                                          span [ _class "hidden-xs" ] [
                                              !!(Icons.mapMarker "") |> withClass "text-subtle"
                                              whitespace
                                              truncate 16 >> encodedText <| game.LocationShort
                                          ]
                                      ])))


                ]
              ]

              sidebar [] [
                  user
                  => fun user ->
                      if user.IsInRole [ Role.Admin
                                         Role.Trener ] then
                          block [] [
                              !!(navList
                                  { Header = "Admin"
                                    Items =
                                        [ { Text =
                                                [ Icons.add ""
                                                  Fable.React.Helpers.str " Ny kamp" ]
                                            Url = "/kamper/ny" } ]
                                    Footer = None
                                    IsSelected = never })
                          ]
                      else
                          empty
                  block [] [
                      !!(navList
                          { Header = "Sesonger"
                            Items =
                                years
                                |> List.map
                                    (fun year ->
                                        { Text = [ (string >> Fable.React.Helpers.str) year ]
                                          Url = listGamesUrl selectedTeam.ShortName year })
                            Footer = None
                            IsSelected = isSelected })
                  ]
              ] ]
            |> layout club user (fun o -> { o with Title = "Kamper" }) ctx
            |> OkResult
