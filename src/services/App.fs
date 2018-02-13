namespace MyTeam

open Giraffe
open Giraffe.GiraffeViewEngine
open Microsoft.AspNetCore.Builder
open MyTeam
open MyTeam.Domain
open MyTeam.Authorization
open System.Linq
open MyTeam.Models.Enums
open MyTeam.Views
open Microsoft.EntityFrameworkCore
open System

module StatsPages = 

    type PlayerStats = {
        Id: Guid
        FacebookId: string
        FirstName: string
        MiddleName: string
        LastName: string
        UrlName: string
        Games: int
        Goals: int
        Assists: int
        YellowCards: int
        RedCards: int
        Image: string
    }   
    with member p.Name = sprintf "%s %s" p.FirstName p.LastName    


    type StatsViewModel = {
        SelectedYear: int 
        Teams: Team list  
        Team: string  
        Years: int list  
        Players: PlayerStats
    }


    let index (club: Club) user (ctx: HttpContext) =

        let db = ctx.Database
        let team = club.Teams |> List.last
        let currentTeam = team.ShortName
        let teamIds = 
                    query {
                        for e in club.Teams |> List.map (fun t -> t.Id) do
                        select e
                    }
        
        let treningskamp = Nullable <| int GameType.Treningskamp
        let gameEvents = db.GameEvents
        let years = 
                query {
                    for gameEvent in gameEvents do
                    where (teamIds.Contains(gameEvent.Game.TeamId) || gameEvent.Game.GameType <> treningskamp)
                    select (gameEvent.Game.DateTime.Year)
                    distinct
                }
                |> Seq.toList
                |> List.sortDescending

        let selectedYear = years |> List.head
   
        let stats =
            let gameIds = 
                    query {
                        for game in db.Games do
                        where (teamIds.Contains(game.TeamId) 
                                 && game.GameType <> treningskamp
                                && game.DateTime.Year = selectedYear)
                        select(game.Id)    
                        distinct                 
                    }
                    |> Seq.toList

            let attendances =
                query {
                    for a in db.EventAttendances do
                    where (gameIds.Contains(a.EventId) && a.IsSelected)
                    select(a.MemberId)    
                    distinct                 
                }        
                |> Seq.toList

            let gameEvents = 
                query { 
                    for ge in db.GameEvents do
                    where (gameIds.Contains(ge.GameId))
                    select (ge)
                }                    
                |> Seq.toList

            let stats = query {
                            for p in db.Players do
                            where (attendances.Contains(p.Id))
                            select (p.Id, p.FacebookId, p.FirstName, p.MiddleName, p.LastName, p.ImageFull, p.UrlName)
                        } |> Seq.toList
                          |> List.map (fun (id, facebookId, firstName, middleName, lastName, imageFull, urlName) ->
                                    {
                                        Id = id
                                        FacebookId = facebookId
                                        FirstName = firstName
                                        MiddleName = middleName
                                        LastName = lastName
                                        UrlName = urlName                                  
                                        Games = attendances |> List.filter (fun a -> a = id) |> Seq.length
                                        Goals = gameEvents |> List.filter(fun ge -> ge.Type = GameEventType.Goal && ge.PlayerId = Nullable id) |> List.length
                                        Assists = gameEvents |> List.filter (fun ge -> ge.Type = GameEventType.Goal && ge.AssistedById = Nullable id) |> List.length
                                        YellowCards = gameEvents |> List.filter (fun ge -> ge.Type = GameEventType.YellowCard && ge.PlayerId = Nullable id) |> List.length
                                        RedCards = gameEvents |> List.filter (fun ge -> ge.Type = GameEventType.RedCard && ge.PlayerId = Nullable id) |> List.length
                                        Image = imageFull
                                    }
                           )
            query {
                 for p in stats do
                 sortBy p.Games
                 thenBy (p.Goals + p.Assists)
                 thenBy p.YellowCards
                 thenBy p.RedCards
             } |> Seq.toList    

        let statsUrl team year = 
            sprintf "/statistikk/%s/%i" team year       

        let getImage = Images.getMember ctx

        [
            div [_class "mt-main"] [
                div [_class "mt-container"] [
                    div [_class "row"] [
                        club.Teams.Length > 1 =?                        
                            (div [_class "stats-teamNav"] [
                                ul [_class "nav nav-pills mt-justified"] 
                                    ((club.Teams 
                                        |> List.map (fun t -> 
                                                        li [_class (currentTeam = t.ShortName =? ("active", ""))] [
                                                            a [_href <| sprintf "/statistikk/%s/%i" t.ShortName selectedYear ] [
                                                                i [_class "hidden-xs flaticon-football43"] [] 
                                                                whitespace
                                                                span [_class "hidden-xs"] [encodedText t.Name] 
                                                                span [_class "visible-xs"] [encodedText t.ShortName]
                                                            ]
                                                        ]                                    
                                    )) @ [ 
                                            li [_class (currentTeam = "total" =? ("active", ""))] [
                                                a [_href <| statsUrl "total" selectedYear ] [encodedText "Samlet" ]
                                            ]
                                        ]
                                    )
                                ]                                                                                                           
                            , emptyText)

                        (years.Length > 0 =?
                            (
                            div [_class "stats-yearNav--mobile"] [
                                select [_class "linkSelect form-control pull-right hidden-md hidden-lg"]  
                                    (List.append 
                                    (years |> List.map (fun y  ->
                                                        option [_value <| statsUrl currentTeam y; selectedYear = y =? (_selected, _empty) ] [
                                                            encodedText <| str y
                                                        ] 
                                                       ))
                                    [ 
                                        option [_value <| statsUrl currentTeam 0; selectedYear = 0 =? (_selected, _empty)] [
                                            encodedText "Total"
                                        ]
                                    ]      
                                    )                                        
                                
                            ]
                            , emptyText))
                    ]
                ]     
                div [_class "mt-container"] [
                    table [_class "table tablesorter stats-table attendance-table"] [
                        thead [] [
                            tr [] [
                                    th [] [encodedText "Spiller"]
                                    th [_class "score"] [Icons.player "Kamper"] 
                                    th [_class "score"] [Icons.goal "Mål"]
                                    th [_class "score"] [Icons.assist "Assists"]
                                    th [_class "score"] [Icons.yellowCard "Gule Kort"]
                                    th [_class "score"] [Icons.redCard "Røde kort" ]
                            ]
                        ]
                        tbody [] 
                                (stats |> List.map(fun player ->
                                                    tr [] [
                                                            td [_class "attendance-player-image"] [
                                                                span [_class "attendance-playerImageContainer"] [
                                                                    img [_src <| getImage player.Image player.FacebookId (fun o -> { o with Height = Some 50; Width = Some 50 })] 
                                                                    whitespace                                                        
                                                                ] 
                                                                a [_href <| sprintf "/spillere/vis/%s" player.UrlName] [
                                                                    encodedText player.Name
                                                                ]
                                                            ]                                                
                                                            td [_class "score"] [encodedText <| str player.Games]
                                                            td [_class "score"] [encodedText <| str player.Goals]
                                                            td [_class "score"] [encodedText <| str player.Assists]
                                                            td [_class "score"] [encodedText <| str player.YellowCards]
                                                            td [_class "score"] [encodedText <| str player.RedCards]
                                                        ]
                                        )
                                )
                    ]
                ]
            ]
            (years.Length > 0 =?
                            (
                            div [_class "mt-sidebar"] [
                                div [_class "mt-container"] [
                                    ul [_class "nav nav-list"]
                                        (
                                        [ li [_class "nav-header"] [encodedText "Sesonger"] ] @   
                                        (years |> List.map (fun year  ->
                                                            li [] [
                                                                a [_href <| statsUrl currentTeam year; selectedYear = year =? (_selected, _empty) ] [
                                                                    encodedText <| str year
                                                                ]
                                                            ] 
                                                           )) @
                                        [ 
                                            li [] [hr [] ]
                                            li [] [a [_href <| statsUrl currentTeam 0;_class (selectedYear = 0 =? ("active", ""))] [
                                                         encodedText "Total"
                                                  ]
                                            ]
                                        ]      
                                        )  
                                                                         
                                ]
                            ]
                            , emptyText))        
        ] 
        |> layout club user (fun o -> { o with Title = "Statistikk"}) ctx
        |> renderHtml



module App =
    let webApp =
        fun next ctx ->
            let (club, user) = Tenant.get ctx
            let mustBeInRole = mustBeInRole user

            match club with
                | Some club ->
                      choose [
                        route "/om" >-> (AboutPages.index club user)          
                        route "/statistikk" >-> (StatsPages.index club user)          
                        route "/api/teams" >-> TeamApi.list club.Id
                        route "/api/players" >-> PlayerApi.list club.Id
                        route "/api/events" >=>                      
                            PUT >=> mustBeInRole [Role.Admin; Role.Trener] >=> 
                                routef "/api/events/%s/description" (parseGuid >> EventApi.setDescription club.Id)
                        
                        subRoute "/api/members" 
                            (choose [ 
                                GET >=> choose [ 
                                    route "" >-> MemberApi.list club.Id
                                    route "/facebookids" >-> MemberApi.getFacebookIds club.Id
                                ]
                                PUT >=> 
                                    mustBeInRole [Role.Admin; Role.Trener] >=> 
                                        choose [ 
                                            routef "/%s/status" (parseGuid >> MemberApi.setStatus club.Id)
                                            routef "/%s/togglerole" (parseGuid >> MemberApi.toggleRole club.Id)
                                            routef "/%s/toggleteam" (parseGuid >> MemberApi.toggleTeam club.Id)
                                        ]       
                                POST >=>  
                                    mustBeInRole [Role.Admin; Role.Trener] >=> 
                                        choose [ 
                                            route "" >=> MemberApi.add club.Id
                                        ]
                            ])

                        subRoute "api/games"
                            (choose [
                                GET >=> 
                                    routef "/%s/squad" (parseGuid >> GameApi.getSquad club.Id)
                                                           
                                POST >=> 
                                    mustBeInRole [Role.Admin; Role.Trener; Role.Skribent] >=> choose [ 
                                        routef "/%s/score/home" (parseGuid >> GameApi.setHomeScore club.Id)
                                        routef "/%s/score/away" (parseGuid >> GameApi.setAwayScore club.Id)                                   
                                    ]
                                    mustBeInRole [Role.Trener] >=> choose [                                
                                        routef "/%s/gameplan" (parseGuid >> GameApi.setGamePlan club.Id)
                                        routef "/%s/gameplan/publish" (parseGuid >> GameApi.publishGamePlan club.Id)
                                    ]
                            ])                                                                                                                                                                                                                       
                       ] next ctx
                | None ->
                    choose [
                       ] next ctx

    let useGiraffe (app : IApplicationBuilder)  =
            app.UseGiraffe webApp


