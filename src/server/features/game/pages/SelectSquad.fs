module MyTeam.Games.Pages.SelectSquad

open Giraffe.ViewEngine
open MyTeam
open Shared
open Shared.Domain
open MyTeam.Views
open MyTeam.Common.Features
open Shared.Domain.Members
open Shared.Components
open Shared.Components.Nav
open MyTeam.Games
open Client.Games.SelectSquad
open MyTeam.Views.BaseComponents
open System.Linq


let view (club: Club) (user: User option) gameId (ctx: HttpContext) =

    let db = ctx.Database
    let (ClubId clubId) = club.Id

    query {
        for game in db.Games do
            where (game.Id = gameId && game.ClubId = clubId)
            groupJoin a in db.EventAttendances on (game.Id = a.EventId) into group

            for a in group.DefaultIfEmpty() do
                select ((game.Id, game.DateTime, game.Location, game.Description, game.IsPublished, game.TeamId), a)
    }
    |> Seq.toList
    |> List.groupBy (fun (game, _) -> game)
    |> List.map (fun ((gameId, date, location, description, squadIsPublished, teamId), attendees) ->
        let attendees =
            attendees
            |> List.map (fun (_, a) -> a)
            |> List.filter (isNull >> not)

        ({ Id = gameId
           Date = date
           TimeString = Shared.Date.formatTime date
           Location = location
           Description = description =?? ""
           TeamId = teamId.Value
           Squad =
             { IsPublished = squadIsPublished
               MemberIds =
                 attendees
                 |> List.filter (fun a -> a.IsSelected)
                 |> List.map (fun m -> m.MemberId)
                 |> Seq.toList } },
         (attendees
          |> List.map (fun a ->
              { MemberId = a.MemberId
                IsAttending = a.IsAttending |> fromNullable
                Message = a.SignupMessage =?? "" }))))
    |> Seq.tryHead
    |> function
        | None -> NotFound
        | Some (game, signups) ->

            let members = Members.list db club.Id

            let recentAttendance = Queries.getRecentAttendance db game.TeamId

            [ Client.clientView
                  clientView
                  { Game = game
                    Signups = signups
                    Members =
                      members
                      |> List.filter (fun m -> m.Details.Status <> Status.Sluttet)
                    ImageOptions = Images.getOptions ctx
                    RecentAttendance = recentAttendance }

              sidebar [] [
                  user
                  => fun user ->
                      if
                          user.IsInRole [
                              Role.Admin
                              Role.Trener
                          ] then
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
              ] ]
            |> layout club user (fun o -> { o with Title = "Laguttak" }) ctx
            |> OkResult
