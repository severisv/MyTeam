module MyTeam.Players.Pages.Insights

open Giraffe.ViewEngine
open MyTeam
open Shared
open Shared.Domain
open MyTeam.Views
open MyTeam.Views.BaseComponents
open Shared.Domain.Members
open System.Linq
open MyTeam.Models.Enums
open MyTeam.Players
open MyTeam.Players.Pages
open MyTeam.Stats
open Shared.Components
open Shared.Components.Nav
open System

let view (club: Club) (user: User option) urlName selectedTeamShortName (ctx: HttpContext) =

    let db = ctx.Database
    let (ClubId clubId) = club.Id

    let urlName = urlName |> Strings.toLower

    let (!!!) = Strings.defaultValue

    // Helper function for winrate color
    let winRateColorClass winRate =
        if winRate < 40.0 then "red"
        elif winRate <= 50.0 then "yellow"
        else "green"

    match db.Members.Where(fun m -> m.ClubId = clubId && m.UrlName = urlName)
          |> Seq.toList
          |> List.map (fun p ->
              {| Id = p.Id
                 FirstName = !!!p.FirstName
                 MiddleName = !!!p.MiddleName
                 LastName = !!!p.LastName
                 FullName = sprintf "%s %s %s" p.FirstName p.MiddleName p.LastName
                 UrlName = !!!p.UrlName
                 Image = !!!p.ImageFull
                 FacebookId = !!!p.FacebookId
                 Status = PlayerStatus.fromInt p.Status |})
          |> List.tryHead
        with
    | None -> NotFound
    | Some player ->

        // Get active players for sidebar
        let players =
            let statusInt = (player.Status |> PlayerStatus.toInt)

            (db.Members.Where(fun m -> m.ClubId = clubId && m.Status = statusInt))
            |> Common.Features.Members.selectMembers
            |> Seq.toList

        // Get teams where player has games (excluding treningskamp)
        let treningskamp = Nullable <| int Models.Enums.GameType.Treningskamp
        let now = DateTime.Now

        let playerTeamIds =
            query {
                for ea in db.EventAttendances do
                    where (
                        ea.Event.GameType <> treningskamp
                        && ea.Event.DateTime < now
                        && ea.MemberId = player.Id
                        && ea.IsSelected
                    )

                    select ea.Event.TeamId.Value
                    distinct
            }
            |> Seq.toList

        let playerTeams =
            club.Teams
            |> List.filter (fun t -> playerTeamIds.Contains(t.Id))

        // Count appearances per team to find the team with most games
        let teamAppearances =
            query {
                for ea in db.EventAttendances do
                    where (
                        ea.Event.GameType <> treningskamp
                        && ea.Event.DateTime < now
                        && ea.MemberId = player.Id
                        && ea.IsSelected
                    )

                    groupBy ea.Event.TeamId.Value into g
                    select (g.Key, g.Count())
            }
            |> Seq.toList
            |> List.sortByDescending snd

        // Determine which league types player has played in
        let playerLeagueTypes =
            playerTeams
            |> List.map (fun t -> t.LeagueType)
            |> List.distinct

        // Select team based on URL parameter
        let selectedTeam =
            match selectedTeamShortName with
            | None ->
                // Select team with most appearances, fallback to first team
                match teamAppearances with
                | (topTeamId, _) :: _ ->
                    playerTeams
                    |> List.tryFind (fun t -> t.Id = topTeamId)
                    |> Option.map MyTeam.Stats.Team
                | [] ->
                    playerTeams
                    |> List.tryHead
                    |> Option.map MyTeam.Stats.Team
            | Some s when (s |> toLower) = "ellever" ->
                let teams =
                    playerTeams
                    |> List.filter (fun t -> t.LeagueType = Ellever)

                if teams.IsEmpty then
                    None
                else
                    Some(MyTeam.Stats.Elleven teams)
            | Some s when (s |> toLower) = "syver" ->
                let teams =
                    playerTeams
                    |> List.filter (fun t -> t.LeagueType = Syver)

                if teams.IsEmpty then
                    None
                else
                    Some(MyTeam.Stats.Seven teams)
            | Some s ->
                playerTeams
                |> List.tryFind (fun t -> t.ShortName |> toLower = (s |> toLower))
                |> Option.map MyTeam.Stats.Team

        match selectedTeam with
        | None -> NotFound
        | Some selectedTeam ->

            let insights = InsightsQueries.get db player.Id selectedTeam

            let insightsUrl team =
                let teamPart =
                    match team with
                    | MyTeam.Stats.Seven _ -> "syver"
                    | MyTeam.Stats.Elleven _ -> "ellever"
                    | MyTeam.Stats.Team t -> t.ShortName

                sprintf "/spillere/vis/%s/innsikt/%s" player.UrlName teamPart

            let isSelected url = insightsUrl selectedTeam = url

            let formatDecimal (value: float) (decimals: int) =
                Math
                    .Round(value, decimals)
                    .ToString(sprintf "F%d" decimals)

            [ mtMain [] [
                  block [ _class "player-insights" ] [
                      !!(Tabs.tabs
                          [ Fable.React.Props.Class "team-nav stats-nav" ]
                          ((playerTeams
                            |> List.map (fun team ->
                                { Text = team.ShortName
                                  ShortText = team.ShortName
                                  Icon = Some <| Icons.team ""
                                  Url = insightsUrl (MyTeam.Stats.Team team) }))
                           @ (playerLeagueTypes
                              |> List.filter (fun leagueType ->
                                  // Only show aggregated tabs if there are multiple teams of this type
                                  let teamsOfType =
                                      playerTeams
                                      |> List.filter (fun t -> t.LeagueType = leagueType)

                                  teamsOfType.Length > 1)
                              |> List.map (fun leagueType ->
                                  let text =
                                      if playerLeagueTypes.Length > 1 then
                                          (match leagueType with
                                           | Syver -> "7'er"
                                           | Ellever -> "11'er")
                                      else
                                          "Samlet"

                                  { Text = sprintf "Samlet %s" text
                                    ShortText = text
                                    Icon = None
                                    Url =
                                      insightsUrl (
                                          match leagueType with
                                          | Syver -> MyTeam.Stats.Seven []
                                          | Ellever -> MyTeam.Stats.Elleven []
                                      ) })))
                          isSelected)

                      hr []
                      br []

                      match insights with
                      | None ->
                          div [ _class "alert alert-info" ] [
                              encodedText "Ingen kampdata tilgjengelig for dette laget."
                          ]
                      | Some data ->
                          div [ _class "row" ] [
                              div [ _class "col-md-6" ] [
                                  div [ _class "panel panel-default" ] [
                                      div [ _class "panel-heading" ] [
                                          h3 [ _class "panel-title" ] [
                                              encodedText "Kampstatistikk"
                                          ]
                                      ]
                                      div [ _class "panel-body" ] [
                                          table [ _class "table table-hover" ] [
                                              tbody [] [
                                                  tr [] [
                                                      td [] [ encodedText "Kamper spilt" ]
                                                      td [ _class "text-right" ] [
                                                          strong [] [
                                                              encodedText <| string data.Games
                                                          ]
                                                      ]
                                                  ]
                                                  tr [] [
                                                      td [] [ encodedText "Vunnet" ]
                                                      td [ _class "text-right" ] [
                                                          encodedText <| string data.Wins
                                                      ]
                                                  ]
                                                  tr [] [
                                                      td [] [ encodedText "Seiersprosent" ]
                                                      td [ _class "text-right" ] [
                                                          strong [ _class <| winRateColorClass data.WinRate ] [
                                                              encodedText
                                                              <| sprintf "%s%%" (formatDecimal data.WinRate 1)
                                                          ]
                                                      ]
                                                  ]
                                                  tr [] [
                                                      td [] [ encodedText "Holdt nullen" ]
                                                      td [ _class "text-right" ] [
                                                          encodedText <| string data.CleanSheets
                                                      ]
                                                  ]
                                                  tr [] [
                                                      td [] [
                                                          encodedText "Gjennomsnittlig sluttresultat"
                                                      ]
                                                      td [ _class "text-right" ] [
                                                          strong [] [
                                                              encodedText
                                                              <| sprintf
                                                                  "%s - %s"
                                                                  (formatDecimal data.AvgGoalsFor 1)
                                                                  (formatDecimal data.AvgGoalsAgainst 1)
                                                          ]
                                                      ]
                                                  ]
                                              ]
                                          ]
                                      ]
                                  ]
                              ]
                              div [ _class "col-md-6" ] [
                                  div [ _class "panel panel-default" ] [
                                      div [ _class "panel-heading" ] [
                                          h3 [ _class "panel-title" ] [
                                              encodedText "Prestasjoner"
                                          ]
                                      ]
                                      div [ _class "panel-body" ] [
                                          table [ _class "table table-hover" ] [
                                              tbody [] [
                                                  tr [] [
                                                      td [] [ encodedText "Mål per kamp" ]
                                                      td [ _class "text-right" ] [
                                                          strong [] [
                                                              encodedText <| formatDecimal data.GoalsPerGame 2
                                                          ]
                                                      ]
                                                  ]
                                                  tr [] [
                                                      td [] [ encodedText "Assists per kamp" ]
                                                      td [ _class "text-right" ] [
                                                          strong [] [
                                                              encodedText <| formatDecimal data.AssistsPerGame 2
                                                          ]
                                                      ]
                                                  ]
                                              ]
                                          ]
                                      ]
                                  ]
                              ]
                          ]

                          // Player Relationships Section
                          match data.Relationships with
                          | None -> emptyText
                          | Some relationships ->
                              div [] [
                                  br []
                                  hr []
                                  br []

                                  h3 [] [ encodedText "Lagkamerater" ]
                                  br []

                                  // Most Played With
                                  div [ _class "row" ] [
                                      div [ _class "col-md-4" ] [
                                          div [ _class "panel panel-default" ] [
                                              div [ _class "panel-heading" ] [
                                                  h4 [ _class "panel-title" ] [
                                                      encodedText "Spilt mest med"
                                                  ]
                                              ]
                                              div [ _class "panel-body" ] [
                                                  if relationships.MostPlayedWith.IsEmpty then
                                                      div [ _class "text-muted" ] [
                                                          encodedText "Ingen data tilgjengelig"
                                                      ]
                                                  else
                                                      table [ _class "table table-condensed" ] [
                                                          tbody [] [
                                                              for stat in relationships.MostPlayedWith do
                                                                  tr [] [
                                                                      td [] [
                                                                          a [ _href
                                                                              <| sprintf "/spillere/vis/%s/innsikt" stat.PlayerUrlName ] [
                                                                              encodedText stat.PlayerName
                                                                          ]
                                                                      ]
                                                                      td [ _class "text-right" ] [
                                                                          strong [] [
                                                                              encodedText <| sprintf "%d kamper" stat.Games
                                                                          ]
                                                                      ]
                                                                  ]
                                                          ]
                                                      ]
                                              ]
                                          ]
                                      ]

                                      // Highest Winrate
                                      div [ _class "col-md-4" ] [
                                          div [ _class "panel panel-default" ] [
                                              div [ _class "panel-heading" ] [
                                                  h4 [ _class "panel-title" ] [
                                                      encodedText "Høyest seiersprosent med"
                                                  ]
                                              ]
                                              div [ _class "panel-body" ] [
                                                  if relationships.HighestWinrate.IsEmpty then
                                                      div [ _class "text-muted" ] [
                                                          encodedText "Ingen data tilgjengelig (min. 5 kamper)"
                                                      ]
                                                  else
                                                      table [ _class "table table-condensed" ] [
                                                          tbody [] [
                                                              for stat in relationships.HighestWinrate do
                                                                  tr [] [
                                                                      td [] [
                                                                          a [ _href
                                                                              <| sprintf "/spillere/vis/%s/innsikt" stat.PlayerUrlName ] [
                                                                              encodedText stat.PlayerName
                                                                          ]
                                                                          br []
                                                                          small [ _class "text-muted" ] [
                                                                              encodedText <| sprintf "%d kamper" stat.Games
                                                                          ]
                                                                      ]
                                                                      td [ _class "text-right" ] [
                                                                          strong [ _class <| winRateColorClass stat.WinRate.Value ] [
                                                                              encodedText
                                                                              <| sprintf "%s%%" (formatDecimal stat.WinRate.Value 1)
                                                                          ]
                                                                      ]
                                                                  ]
                                                          ]
                                                      ]
                                              ]
                                          ]
                                      ]

                                      // Lowest Winrate
                                      div [ _class "col-md-4" ] [
                                          div [ _class "panel panel-default" ] [
                                              div [ _class "panel-heading" ] [
                                                  h4 [ _class "panel-title" ] [
                                                      encodedText "Lavest seiersprosent med"
                                                  ]
                                              ]
                                              div [ _class "panel-body" ] [
                                                  if relationships.LowestWinrate.IsEmpty then
                                                      div [ _class "text-muted" ] [
                                                          encodedText "Ingen data tilgjengelig (min. 5 kamper)"
                                                      ]
                                                  else
                                                      table [ _class "table table-condensed" ] [
                                                          tbody [] [
                                                              for stat in relationships.LowestWinrate do
                                                                  tr [] [
                                                                      td [] [
                                                                          a [ _href
                                                                              <| sprintf "/spillere/vis/%s/innsikt" stat.PlayerUrlName ] [
                                                                              encodedText stat.PlayerName
                                                                          ]
                                                                          br []
                                                                          small [ _class "text-muted" ] [
                                                                              encodedText <| sprintf "%d kamper" stat.Games
                                                                          ]
                                                                      ]
                                                                      td [ _class "text-right" ] [
                                                                          strong [ _class <| winRateColorClass stat.WinRate.Value ] [
                                                                              encodedText
                                                                              <| sprintf "%s%%" (formatDecimal stat.WinRate.Value 1)
                                                                          ]
                                                                      ]
                                                                  ]
                                                          ]
                                                      ]
                                              ]
                                          ]
                                      ]
                                  ]

                                  // Assist relationships row
                                  div [ _class "row" ] [
                                      div [ _class "col-md-4" ] [
                                          div [ _class "panel panel-default" ] [
                                              div [ _class "panel-heading" ] [
                                                  h4 [ _class "panel-title" ] [
                                                      encodedText "Flest assists til"
                                                  ]
                                              ]
                                              div [ _class "panel-body" ] [
                                                  if relationships.MostAssistsTo.IsEmpty then
                                                      div [ _class "text-muted" ] [
                                                          encodedText "Ingen data tilgjengelig"
                                                      ]
                                                  else
                                                      table [ _class "table table-condensed" ] [
                                                          tbody [] [
                                                              for stat in relationships.MostAssistsTo do
                                                                  tr [] [
                                                                      td [] [
                                                                          a [ _href
                                                                              <| sprintf "/spillere/vis/%s/innsikt" stat.PlayerUrlName ] [
                                                                              encodedText stat.PlayerName
                                                                          ]
                                                                      ]
                                                                      td [ _class "text-right" ] [
                                                                          strong [] [
                                                                              encodedText
                                                                              <| sprintf "%d assists" stat.Assists.Value
                                                                          ]
                                                                      ]
                                                                  ]
                                                          ]
                                                      ]
                                              ]
                                          ]
                                      ]

                                      div [ _class "col-md-4" ] [
                                          div [ _class "panel panel-default" ] [
                                              div [ _class "panel-heading" ] [
                                                  h4 [ _class "panel-title" ] [
                                                      encodedText "Flest assists fra"
                                                  ]
                                              ]
                                              div [ _class "panel-body" ] [
                                                  if relationships.MostAssistsFrom.IsEmpty then
                                                      div [ _class "text-muted" ] [
                                                          encodedText "Ingen data tilgjengelig"
                                                      ]
                                                  else
                                                      table [ _class "table table-condensed" ] [
                                                          tbody [] [
                                                              for stat in relationships.MostAssistsFrom do
                                                                  tr [] [
                                                                      td [] [
                                                                          a [ _href
                                                                              <| sprintf "/spillere/vis/%s/innsikt" stat.PlayerUrlName ] [
                                                                              encodedText stat.PlayerName
                                                                          ]
                                                                      ]
                                                                      td [ _class "text-right" ] [
                                                                          strong [] [
                                                                              encodedText
                                                                              <| sprintf "%d assists" stat.Assists.Value
                                                                          ]
                                                                      ]
                                                                  ]
                                                          ]
                                                      ]
                                              ]
                                          ]
                                      ]

                                      div [ _class "col-md-4" ] [
                                          div [ _class "panel panel-default" ] [
                                              div [ _class "panel-heading" ] [
                                                  h4 [ _class "panel-title" ] [
                                                      encodedText "Bestevenner"
                                                  ]
                                              ]
                                              div [ _class "panel-body" ] [
                                                  if relationships.BestFriends.IsEmpty then
                                                      div [ _class "text-muted" ] [
                                                          encodedText "Ingen data tilgjengelig"
                                                      ]
                                                  else
                                                      table [ _class "table table-condensed" ] [
                                                          tbody [] [
                                                              for stat in relationships.BestFriends do
                                                                  tr [] [
                                                                      td [] [
                                                                          a [ _href
                                                                              <| sprintf "/spillere/vis/%s/innsikt" stat.PlayerUrlName ] [
                                                                              encodedText stat.PlayerName
                                                                          ]
                                                                      ]
                                                                      td [ _class "text-right" ] [
                                                                          strong [] [
                                                                              encodedText
                                                                              <| sprintf "%d assists" stat.Assists.Value
                                                                          ]
                                                                      ]
                                                                  ]
                                                          ]
                                                      ]
                                              ]
                                          ]
                                      ]
                                  ]
                              ]
                  ]
              ]

              // Add sidebar with active players (links to insights pages)
              Common.sidebar player.Status players player.UrlName Common.showPlayerInsightsUrl

              emptyText ]
            |> layout club user (fun o -> { o with Title = player.FullName }) ctx
            |> OkResult
