module MyTeam.Games.Pages.Show

open Giraffe.GiraffeViewEngine
open MyTeam
open Shared
open Server
open Shared.Domain
open MyTeam.Games
open MyTeam.Views
open Shared.Domain.Members
open System
open Shared.Components
open MyTeam.Views.BaseComponents

let view (club: Club) (user: Users.User option) gameId (ctx: HttpContext) =

    let db = ctx.Database

    Queries.getGame db club.Id gameId
    |> function
    | None -> NotFound
    | Some game ->    

        let matchReport =
            game.MatchReportName 
            |> Option.map (Common.News.Queries.getArticle db club.Id)
            |> Option.defaultValue None      

        let gameHasPassed = game.DateTime < DateTime.Now

        [
            mtMain [] [      
                div [ _class "mt-container" ] [  
                    user => fun user -> 
                            if user.IsInRole [Role.Admin;Role.Trener] then
                                 !!(editLink <| sprintf "/intern/arrangement/endre/%O" game.Id)
                            else empty     
                    
                    user => fun user -> 
                            if user.IsInRole [Role.Admin;Role.Trener;Role.Skribent] && gameHasPassed then
                                    a [ _href <| sprintf "/kamper/%O/resultat" game.Id ;_class "edit-link pull-right"][ 
                                        !!(Icons.ballInGoal "Registrer resultat")
                                    ]
                            else empty

                    user => fun user -> 
                            if user.IsInRole [Role.Trener] || game.GamePlanIsPublished then
                                    a [ _href <| sprintf "/kamper/%O/bytteplan" game.Id ;_class "registerSquad-gameplan-link pull-right"][ 
                                        !!Icons.gamePlan
                                    ]
                            else empty                            
                                         
                    h3 [ _class "game-header hidden-xs" ] [  
                        encodedText game.HomeTeam  
                        span [ _class "game-score" ] [  
                                encodedText <| sprintf "%s - %s" 
                                    (game.HomeScore |> Option.map string |> Option.defaultValue "") 
                                    (game.AwayScore |> Option.map string |> Option.defaultValue "") 
                            ]
                        encodedText game.AwayTeam
                    ]
                
                    Giraffe.GiraffeViewEngine.table [ _class "game-scoreTable visible-xs" ] [ 
                        tr [] [ 
                            td [] [  encodedText game.HomeTeam ]
                            td [ _class "game-scoreTable--score" ][ game.HomeScore |> Option.map(string >> encodedText) |> Option.defaultValue empty ]
                            tr [][ 
                                td [] [ encodedText game.AwayTeam ]
                                td [ _class "game-scoreTable--score" ]
                                    [  game.AwayScore |> Option.map(string >> encodedText) |> Option.defaultValue empty ]
                                ]
                            ]
                    ]
                    hr []
                    div [ _class "row game-details" ][ 
                        div [ _class "col-sm-9 col-sm-offset-2 col-xs-11 col-xs-offset-1 no-padding" ][ 
                            div [ _class "col-sm-5" ] [ 
                                p [ _class "col-sm-3 col-xs-2 game-details-icon" ][ 
                                    !!(Icons.calendar "")
                                     ]
                                p [ _class "col-sm-9" ] [
                                    (game.DateTime.Year < DateTime.Now.Year =? 
                                        (Date.formatLong, Date.format)) 
                                            game.DateTime |> encodedText 
                                ]                                           
                                p [ _class "col-sm-3 col-xs-2 game-details-icon" ]
                                    [ 
                                        !!(Icons.time "Tidspunkt")
                                    ]
                                p [ _class "col-sm-9" ]
                                    [ game.DateTime |> Date.formatTime |> encodedText ]
                                ]
                            div [ _class "col-sm-7" ][ 
                                p [ _class "col-sm-3 col-xs-2 game-details-icon" ][ 
                                    !!(Icons.gameType game.Type)
                                    ]
                                p [ _class "col-sm-8" ]
                                    [  encodedText <| string game.Type ]
                                p [ _class "col-sm-3 col-xs-2 game-details-icon" ]
                                    [
                                        !!(Icons.mapMarker "Sted")
                                    ]
                                p [ _class "col-sm-8" ]
                                    [  encodedText game.Location ]
                                ]
                            ]  
                    ]         
                    gameHasPassed =? 
                        (div [ 
                            _id "game-showEvents" 
                            attr "data-game-id" (string game.Id) 
                            attr "data-show-player-url" "/spillere/vis"
                            attr "data-edit-mode" "false" ][], empty)             
                ]                
           
                matchReport => fun matchReport -> 
                        block [ _class "u-fade-in-on-enter" ] [
                            h2 [ _class "news-matchReport" ] [ encodedText "Kamprapport" ]
                            hr [ ]
                            Common.News.Components.showArticle ctx user matchReport None
                        ]                  
            ] 
        ]
        |> layout club user (fun o -> { o with 
                                            Title = "Kampdetaljer"
                                        }
                            ) ctx
        |> OkResult
