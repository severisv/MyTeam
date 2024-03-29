module MyTeam.Games.Pages.GamePlan

open MyTeam
open Shared
open MyTeam.Common.Features
open Shared.Domain
open Shared.Domain.Members
open Client.GamePlan.Formation
open Client.GamePlan.View
open System
open System.Linq


let view (club: Club) (user: User) gameId (ctx: HttpContext) =

    let db = ctx.Database
    let (ClubId clubId) = club.Id

    query {
        for game in db.Games do
            where (game.Id = gameId && game.ClubId = clubId)

            select (
                game.Opponent,
                game.GamePlanState,
                game.GamePlanIsPublished,
                game.TeamId,
                game.Team.Formation,
                query {
                    for attendee in game.Attendees do
                        where (attendee.IsSelected)
                        select (attendee.Member)
                }
            )
    }
    |> Seq.map (fun (opponent, gamePlan, gamePlanIsPublished, teamId, formation, attendees) ->
        ({ GameId = gameId
           Team =
             club.Teams
             |> List.find (fun t -> t.Id = teamId.Value)
             |> fun t -> t.ShortName
           Opponent = opponent
           GamePlanIsPublished = (gamePlanIsPublished = Nullable(true))
           GamePlan =
             if isNull gamePlan then
                 None
             else
                 (Json.fableDeserialize<GamePlanState> gamePlan
                  |> function
                      | Ok g -> Some g
                      | Error e -> failwithf "%O" e)
           Players =
             Members.selectMembers (attendees.AsQueryable())
             |> Seq.toList
           ImageOptions = Images.getOptions ctx
           Formation =
             match int formation with
             | 0 -> Ellever FourThreeThree
             | 433 -> Ellever FourThreeThree
             | 442 -> Ellever FourFourTwo
             | 532 -> Ellever FiveThreeTwo
             | 321 -> Sjuer ThreeTwoOne
             | 231 -> Sjuer TwoThreeOne
             | _ -> failwithf "Ukjent formasjon %O" formation }))
    |> Seq.tryHead
    |> function
        | Some model when
            model.GamePlanIsPublished || user.IsInRole [ Role.Trener ] || user.UserId = "severin@sverdvik.no"
            ->
            [ Client.clientView clientView model ]
            |> layout club (Some user) (fun o -> { o with Title = "Bytteplan" }) ctx
            |> OkResult
        | _ -> NotFound
