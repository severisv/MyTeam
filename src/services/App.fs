namespace MyTeam

open Giraffe
open Giraffe.GiraffeViewEngine
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open MyTeam
open MyTeam.Domain
open MyTeam.Authorization
open MyTeam.Views
open MyTeam.Models.Enums
open System.Linq
open System



module App =
    let webApp =
        fun next ctx ->
            let (club, user) = Tenant.get ctx
            let mustBeInRole = mustBeInRole user


            match club with
                | Some club ->
                      choose [
                        route "/om" >-> (AboutPages.index club user)          
                        route "/statistikk" >=> StatsPages.index club user None None      
                        routef "/statistikk/%s/%s" (fun (teamName, year) -> StatsPages.index club user (Some teamName) (Some year))          
                        routef "/statistikk/%s" (fun teamName -> StatsPages.index club user (Some teamName) None)      
                        subRoute "/intern" 
                            (user |> Option.fold 
                                        (fun _ user ->
                                                (choose [ 
                                                    GET >=> choose [ 
                                                        route "/oppmote" >=> AttendancePages.index club user None
                                                        routef "/oppmote/%s" (fun year -> AttendancePages.index club user (Some <| toLower year))
                                                    ]                                
                                                ])
                                        )                        
                                        empty
                            )                           
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

    let addGiraffe (services : IServiceCollection)  =
        services.AddGiraffe() |> ignore


