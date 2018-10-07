module MyTeam.Games.Pages.SelectSquad

open Giraffe.GiraffeViewEngine
open MyTeam
open MyTeam.Domain
open MyTeam.Views
open MyTeam.Domain.Members
open MyTeam.Shared.Components
open Shared.Features.Games.SelectSquad

let view (club: Club) (user: Users.User option) gameId (ctx: HttpContext) =

    let db = ctx.Database

    let (ClubId clubId) = club.Id 
    query {
        for game in db.Games do
        where (game.Id = gameId && game.ClubId = clubId)
        select (game.DateTime, game.Location, game.Description, game.IsPublished, game.Attendees)
    }
    |> Seq.map (fun (date, location, description, squadIsPublished, attendees) ->
                ({
                    Id = gameId
                    Date = date
                    Location = location
                    Description = description =?? ""
                    Squad = {
                                IsPublished = squadIsPublished
                                MemberIds = attendees 
                                            |> Seq.filter(fun a -> a.IsSelected)
                                            |> Seq.map(fun m -> m.Id) |> Seq.toList
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


        [
            Client.view clientView 
                        {
                            Game = game
                            Signups = signups
                            ImageOptions = Images.getOptions ctx
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

