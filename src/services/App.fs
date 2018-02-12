namespace MyTeam

open Giraffe
open Giraffe.GiraffeViewEngine
open Microsoft.AspNetCore.Builder
open MyTeam
open MyTeam.Domain
open MyTeam.Authorization
open System.Linq
open MyTeam.Models.Enums
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

        [
            div [_class "mt-main"] [
                div [_class "mt-container"] [
                    div [_class "row"] [
                        club.Teams.Length > 1 =?                        
                            (div [_class "stats-teamNav"] [
                                ul [_class "nav nav-pills mt-justified"] 
                                    (club.Teams |> List.map (fun t -> 
                                                            li [_class (currentTeam = t.ShortName =? ("active", ""))] [
                                                                a [_href <| sprintf "/statistikk/%s/%i" t.ShortName selectedYear ] [
                                                                    i [_class "hidden-xs flaticon-football43"] [] 
                                                                    span [_class "hidden-xs"] [encodedText t.Name] 
                                                                    span [_class "visible-xs"] [encodedText t.ShortName]
                                                                ]
                                                            ]                                    
                                                            ) 
                                    |> List.append [ 
                                                    li [_class (currentTeam = "total" =? ("active", ""))] [
                                                        a [_href <| sprintf "/statistikk/total/%i" selectedYear ] [encodedText "Samlet"]
                                                    ]])    
                                ]                                                                                                           
                            , emptyText)
                            ]
                    ]
                ]
                        
            //             @if (Model.Years.Any())
            //             {
            //                 div [_class "stats-yearNav--mobile"] [

            //                         <select class "linkSelect form-control pull-right hidden-md hidden-lg"] [
            //                             @foreach (var year in Model.Years)
            //                             {
            //                                 <option value="@Url.Action("Index", "Stats", new {lag = Model.Team, aar = year})" selected="@(Model.SelectedYear == year)"] [@year</option] [
            //                             }
            //                             <option value="@Url.Action("Index", "Stats", new {lag = Model.Team, aar = 0})" selected="@(Model.SelectedYear == 0)" ] [Total</option] [
            //                         </select] [
            //                     ]
            //             }
            //         ]
            //     ]

            //     div [_class "mt-container"] [
            //         <table class "table tablesorter stats-table attendance-table"] [
            //             <thead] [
            //             <tr] [
            //                 <th] [@Res.Player</th] [
            //                 th [_class "score"] [<i title="Kamper" class "@Icons.Player"] [</th] [
            //                 th [_class "score"] [<i title="Mål" class "@Icons.Goal"] [</th] [
            //                 th [_class "score"] [<i title="Assists" class "@Icons.Assist"] [</th] [
            //                 th [_class "score"] [<span] [<i title="Gule kort" class "@Icons.YellowCard"] [</span] [</th] [
            //                 th [_class "score"] [<span] [<i title="Røde kort" class "@Icons.RedCard"] [</span] [</th] [
            //             </tr] [
            //             </thead] [
            //             <tbody] [
            //             @foreach (var player in Model.Players)
            //             {
            //                 <tr] [
            //                     <td class "attendance-player-image"] [
            //                         <span class "attendance-playerImageContainer"] [
            //                             <img src="@Context.Cloudinary().MemberImage(player.Image, player.FacebookId, 50, 50)"/] [&nbsp;
            //                         </span] [
            //                         <a asp-controller="player" asp-action="show" asp-route-name="@player.UrlName"] [
            //                             @player.Name
            //                         ]
            //                     ]
            //                     <td class "score"] [@player.Games]
            //                     <td class "score"] [@player.Goals]
            //                     <td class "score"] [@player.Assists]
            //                     <td class "score"] [@player.YellowCards]
            //                     <td class "score"] [@player.RedCards]
            //                 </tr] [
            //             }
            //             </tbody] [
            //         </table] [
            //     ]
            // ]





            // @if (Model.Years?.Any() == true)
            // {
            //     div [_class "mt-sidebar"] [
            //         div [_class "mt-container"] [
            //             ul [_class "nav nav-list"] [
            //                 li [_class "nav-header"] [@Res.Season.Pluralize()]
            //                 @foreach (var year in Model.Years)
            //                 {
            //                     <li] [<a class "@(Model.SelectedYear == year ? "active" : "")" asp-controller="stats" asp-action="index" asp-route-lag="@Model.Team" asp-route-aar="@year"] [@year]]
            //                 }
            //                 <li] [<hr /] []
            //                 <li] [<a class "@(Model.SelectedYear == 0 ? "active" : "")" asp-controller="stats" asp-action="index" asp-route-lag="@Model.Team" asp-route-aar="0" ] [Total]]
            //             ]

            //         ]
            //         ]
            //         }

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


