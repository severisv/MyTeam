module MyTeam.Games.Pages.GamePlan

open MyTeam
open Shared
open MyTeam.Common.Features
open Shared.Domain
open Shared.Domain.Members
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
                 game.Team.Formation,
                 query {                     
                    for attendee in game.Attendees do
                        where(attendee.IsSelected)
                        select(attendee.Member)
                 }                 
             )
    }
    |> Seq.map(fun (opponent, gamePlan, gamePlanIsPublished, teamId, formation, attendees) -> 
           ({ GameId = gameId
              Team = club.Teams 
                     |> Seq.find(fun t -> t.Id = teamId) 
                     |> fun t -> t.ShortName
              Opponent = opponent
              GamePlanIsPublished = (gamePlanIsPublished = Nullable(true))
              GamePlan = Strings.asOption gamePlan
              Players = Members.selectMembers (attendees.AsQueryable()) |> Seq.toList
              ImageOptions = Images.getOptions ctx
              Formation = match int formation with
                          | 0 -> Ellever FourThreeThree
                          | 433 -> Ellever FourThreeThree
                          | 442 -> Ellever FourFourTwo
                          | 321 -> Sjuer ThreeTwoOne
                          | 231 -> Sjuer TwoThreeOne
                          | _ -> failwithf "Ukjent formasjon %O" formation
               })
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
