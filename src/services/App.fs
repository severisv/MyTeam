namespace MyTeam

open Giraffe
open Microsoft.AspNetCore.Builder
open MyTeam
open MyTeam.Domain
open MyTeam.Authorization

module App =

    let webApp =
        fun next ctx ->
            let (club, user) = Tenant.get ctx
            let mustBeInRole = mustBeInRole user

            match club with
                | Some club ->
                      choose [
                        GET >=> route "/api/teams" >-> TeamApi.list club.Id
                        GET >=> route "/api/players" >-> PlayerApi.list club.Id                      
                        PUT >=> mustBeInRole [Role.Admin; Role.Trener] >=> 
                            choose [ 
                                routef "/api/events/%s/description" (parseGuid >> EventApi.setDescription club.Id)
                            ]
                        GET >=> 
                            choose [ 
                                route "/api/members" >-> MemberApi.list club.Id
                                route "/api/members/facebookids" >-> MemberApi.getFacebookIds club.Id
                            ]
                        mustBeInRole [Role.Admin; Role.Trener] >=> 
                            PUT >=> 
                                choose [ 
                                    routef "/api/members/%s/status" (parseGuid >> MemberApi.setStatus club.Id)
                                    routef "/api/members/%s/togglerole" (parseGuid >> MemberApi.toggleRole club.Id)
                                    routef "/api/members/%s/toggleteam" (parseGuid >> MemberApi.toggleTeam club.Id)
                                ]       
                            POST >=>  
                                choose [ 
                                    route "/api/members" >=> MemberApi.add club.Id
                                ]
                        GET >=> 
                            choose [
                                routef "/api/games/%s/squad" (parseGuid >> GameApi.getSquad club.Id)
                            ]                             
                        mustBeInRole [Role.Admin; Role.Trener; Role.Skribent] >=> 
                            POST >=> 
                                choose [ 
                                    routef "/api/games/%s/score/home" (parseGuid >> GameApi.setHomeScore club.Id)
                                    routef "/api/games/%s/score/away" (parseGuid >> GameApi.setAwayScore club.Id)                                   
                                ]     
                        mustBeInRole [Role.Trener] >=> 
                            POST >=> 
                                choose [                                
                                    routef "/api/games/%s/gameplan" (parseGuid >> GameApi.setGamePlan club.Id)
                                    routef "/api/games/%s/gameplan/publish" (parseGuid >> GameApi.publishGamePlan club.Id)
                                ]                                                                                                                                                                               
                       ] next ctx
                | None ->
                    choose [
                        GET >=> route "/api/players" >=> text "No club"
                       ] next ctx

    let useGiraffe (app : IApplicationBuilder)  =
            app.UseGiraffe webApp


