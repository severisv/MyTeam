module MyTeam.Games.Pages.SelectSquad

open Giraffe.GiraffeViewEngine
open MyTeam
open Shared
open Shared.Domain
open MyTeam.Views
open MyTeam.Common.Features
open Shared.Domain.Members
open Shared.Components
open MyTeam.Games
open Shared.Features.Games.SelectSquad
open MyTeam.Views.BaseComponents


let view (club: Club) (user: Users.User option) gameId (ctx: HttpContext) =

    let db = ctx.Database

    let (ClubId clubId) = club.Id 
    query {
        for game in db.Games do
        where (game.Id = gameId && game.ClubId = clubId)
        select (game.DateTime, game.Location, game.Description, game.IsPublished, game.TeamId, game.Attendees)
    }
    |> Seq.map (fun (date, location, description, squadIsPublished, teamId, attendees) ->
                ({
                    Id = gameId
                    Date = date
                    Location = location
                    Description = description =?? ""
                    TeamId = teamId
                    Squad = {
                                IsPublished = squadIsPublished
                                MemberIds = attendees 
                                            |> Seq.filter(fun a -> a.IsSelected)
                                            |> Seq.map(fun m -> m.MemberId) |> Seq.toList
                    }
                }, 
                attendees 
                |> Seq.map (fun a ->
                                if a.IsAttending.HasValue then
                                    Some {
                                        MemberId = a.MemberId
                                        IsAttending = a.IsAttending.Value
                                        Message = a.SignupMessage =?? ""
                                    }
                                else None
                            )          
                |> Seq.choose id
                |> Seq.toList
                )                    
        )
    |> Seq.tryHead
    |> function
    | None -> NotFound
    | Some (game, signups) -> 

        let members = Members.list db club.Id
        let recentAttendance = Queries.getRecentAttendance db game.TeamId
        
        [
            Client.view clientView 
                        {
                            Game = game
                            Signups = signups
                            Members = members |> List.filter(fun m -> m.Details.Status <> Status.Sluttet)
                            ImageOptions = Images.getOptions ctx
                            RecentAttendance = recentAttendance
                        }
                                   
            sidebar [] [
                user =>
                    fun user -> 
                        if user.IsInRole [Role.Admin;Role.Trener] then
                            block [] [ 
                                navList 
                                    {
                                        Header = "Adminmeny"
                                        Items = 
                                            [
                                                { Text = [ !!(Icons.add ""); encodedText " Legg til kamp" ]; Url = "/intern/arrangement/ny?type=Kamp" }
                                            ]                            
                                        Footer = None
                                        IsSelected = never
                                    }    
                            ]
                        else empty                        
            ]
        ]
        |> layout club user (fun o -> { o with Title = "Laguttak" }) ctx
        |> OkResult

