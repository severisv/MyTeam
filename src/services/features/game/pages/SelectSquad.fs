module MyTeam.Games.Pages.SelectSquad

open Giraffe.GiraffeViewEngine
open MyTeam
open MyTeam.Domain
open MyTeam.Shared
open MyTeam.Views
open MyTeam.Domain.Members
open MyTeam.Shared.Components
open System

let view (club: Club) (user: Users.User option) gameId (ctx: HttpContext) =

    let db = ctx.Database
    
    [
        div [_id ClientViews.selectSquad] []
       
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

