module MyTeam.Games.Pages.Result

open Giraffe.GiraffeViewEngine
open MyTeam
open Shared
open Shared.Domain
open MyTeam.Games
open MyTeam.Views
open Shared.Domain.Members
open Shared.Components
open System
open MyTeam.Views.BaseComponents


let view (club: Club) (user: Users.User option) gameId (ctx: HttpContext) =

    let db = ctx.Database

    Queries.getGame db club.Id gameId
    |> function
    | None -> NotFound
    | Some game ->         
        [
            mtMain [] [
                    block [ _class "registerResult" ] [
                        a [ _href <| sprintf "/kamper/vis/%O" game.Id; _class "pull-right"; _title "Vis kamp" ] [ 
                            !!(Icons.arrowLeft "")
                            span [ _class "hidden-xxs" ] [ encodedText "Tilbake" ]
                        ]                                
                        div [ _class "game-header" ][ 
                            span [ _class "registerResult-teamScore" ] [ 
                                encodedText game.HomeTeam
                                input [ 
                                    _type "tel"
                                    _class "registerResult-score ajax-update"
                                    attr "data-href" <| sprintf "/api/games/%O/score/home" game.Id
                                    _value (toString game.HomeScore) 
                                ]
                            ]
                            span [ _class "hidden-xs" ] [ encodedText "-" ]
                            span [ _class "registerResult-teamScore" ] [ 
                                input [ 
                                    _type "tel"
                                    _class "registerResult-score ajax-update" 
                                    attr "data-href" <| sprintf "/api/games/%O/score/away" game.Id
                                    _value (toString game.AwayScore) 
                                ] 
                                encodedText game.AwayTeam
                            ]
                        ]
                        hr []
                        div [ _class "row" ][ 
                            div [ _class "col-sm-6" ]
                                [ p [ _class "col-sm-4 col-xs-2" ]
                                    [ i [ _class "fa fa-13x fa-calendar pull-right" ]
                                        []
                                    ]
                                  p [ _class "col-sm-8" ]
                                    [ encodedText <| Date.formatLong game.DateTime ]
                                  p [ _class "col-sm-4 col-xs-2" ] [ 
                                      !!(Icons.time "") |> withClass "fa fa-13x pull-right"
                                    ]
                                  p [ _class "col-sm-8" ]
                                    [ encodedText <|  Date.formatTime game.DateTime ]
                                ]
                            div [ _class "col-sm-6" ]
                                [ p [ _class "col-sm-4 col-xs-2" ] [ 
                                    !!(Icons.gameType game.Type) |> withClass "pull-right"
                                    ]
                                  p [ _class "col-sm-8" ]
                                    [ encodedText <| string game.Type ]
                                  p [ _class "col-sm-4 col-xs-2" ] [ 
                                      !!(Icons.mapMarker "") |> withClass "fa-13x pull-right"
                                    ]
                                  p [ _class "col-sm-8" ]
                                    [ encodedText game.Location ]
                                ]
                            ]
                        div [ _id "registerResult-addEvent"; attr "data-game-id" (string game.Id) ] []    
                ]
            ]          
           
        ]
        |> layout club user (fun o -> { o with Title = "Kamper" }) ctx
        |> OkResult

