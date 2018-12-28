module MyTeam.Games.Pages.GamePlan

open MyTeam
open MyTeam.Common.Features
open MyTeam.Domain
open MyTeam.Domain.Members
open Shared.Features.Games.GamePlan
open System
open System.Linq
let view (club : Club) (user : Users.User) gameId (ctx : HttpContext) =

    let db = ctx.Database
    let (ClubId clubId) = club.Id
    query { 
        for game in db.Games do
            where(game.Id = gameId && game.ClubId = clubId)
            select
                (game.Opponent, 
                 game.GamePlanState, 
                 game.GamePlanIsPublished, 
                 game.TeamId, 
                 query {                     
                    for attendee in game.Attendees do
                        where(attendee.IsSelected)
                        select(attendee.Member)
                 }                 
             )
    }
    |> Seq.map(fun (opponent, gamePlan, gamePlanIsPublished, teamId, attendees) -> 
           ({ GameId = gameId
              Team = club.Teams 
                     |> Seq.find(fun t -> t.Id = teamId) 
                     |> fun t -> t.ShortName
              Opponent = opponent
              GamePlanIsPublished = (gamePlanIsPublished = Nullable(true))
              GamePlan = Strings.asOption gamePlan
              Players = Members.selectMembers (attendees.AsQueryable()) |> Seq.toList
              ImageOptions = Images.getOptions ctx })
    )                
    |> Seq.tryHead
    |> function 
    | None -> NotFound
    | Some model when (not model.GamePlanIsPublished) && 
                          not <| user.IsInRole [Role.Trener]
                          -> NotFound
    | Some model -> 
        [ Client.view clientView model ]
        |> layout club (Some user) (fun o -> { o with Title = "Bytteplan" }) ctx
        |> OkResult
