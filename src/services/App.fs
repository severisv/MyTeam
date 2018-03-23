namespace MyTeam

open Giraffe
open Giraffe.Serialization
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open MyTeam
open MyTeam.Domain.Members
open MyTeam.Authorization
open Newtonsoft.Json
open Newtonsoft.Json.Converters
open Results

    
module App =

    let webApp =
        removeTrailingSlash >=>
        fun next ctx ->
            let (club, user) = Tenant.get ctx
            let mustBeInRole = mustBeInRole user
            
            match club with
            | Some club ->
                  choose [
                    route "/om" >=> (AboutPages.index club user |> htmlGet)          
                    route "/statistikk" >=> (StatsPages.index club user None None |> htmlGet)      
                    routef "/statistikk/%s/%s" (fun (teamName, year) -> StatsPages.index club user (Some teamName) (Some year) |> htmlGet)          
                    routef "/statistikk/%s" (fun teamName -> StatsPages.index club user (Some teamName) None |> htmlGet)      
                    subRoute "/intern" 
                        mustBeMember >=>
                            (user |> Option.fold 
                                        (fun _ user ->
                                                (choose [ 
                                                    GET >=> mustBeInRole [Role.Admin; Role.Trener; Role.Oppmøte] >=> choose [ 
                                                        route "/oppmote/registrer" >=> (Attendance.Pages.Register.view club user None |> htmlGet)
                                                        routef "/oppmote/registrer/%O" (fun eventId -> Attendance.Pages.Register.view club user (Some eventId) |> htmlGet)                                                       
                                                    ]
                                                    GET >=> choose [                                                        
                                                        route "/oppmote" >=> (Attendance.Pages.Show.view club user None |> htmlGet)
                                                        routef "/oppmote/%s" (fun year -> Attendance.Pages.Show.view club user (Some <| toLower year) |> htmlGet)
                                                        route "/lagliste" >=> (Members.Pages.List.view club user None |> htmlGet)
                                                        routef "/lagliste/%s" (fun status -> Members.Pages.List.view club user (Some status) |> htmlGet)
                                                    ]                                    
                                                ])
                                        )                        
                                        empty
                        )
                    subRoute "/api/attendance"                            
                        (choose [ 
                            GET >=> routef "/%O/recent" (Attendance.Api.getRecentAttendance club >> jsonGet)
                            POST >=> mustBeInRole [Role.Admin; Role.Trener; Role.Oppmøte] >=> 
                                routef "/%O/registrer/%O" (Attendance.Api.confirmAttendance club.Id >> jsonPost)
                                                           
                        ])                                     
                                                    
                    route "/api/teams" >=> (Teams.Api.list club.Id |> jsonGet)
                    route "/api/events" >=>                      
                        PUT >=> mustBeInRole [Role.Admin; Role.Trener] >=> 
                            routef "/api/events/%O/description" (Events.Api.setDescription club.Id)
                    
                    subRoute "/api/members" 
                        (choose [ 
                            GET >=> choose [ 
                                route "" >=> (Members.Api.list club.Id |> jsonGet)
                                route "/facebookids" >=> (Members.Api.getFacebookIds club.Id |> jsonGet)
                            ]
                            PUT >=> 
                                mustBeInRole [Role.Admin; Role.Trener] >=> 
                                    choose [ 
                                        routef "/%O/status" (Members.Api.setStatus club.Id)
                                        routef "/%O/togglerole" (Members.Api.toggleRole club.Id)
                                        routef "/%O/toggleteam" (Members.Api.toggleTeam club.Id >> jsonPost)
                                    ]       
                            POST >=>  
                                mustBeInRole [Role.Admin; Role.Trener] >=> 
                                    choose [ 
                                        route "" >=> (Members.Api.add club.Id |> jsonPost)
                                    ]
                        ])

                    subRoute "/api/games"
                        (choose [
                            POST >=> 
                                mustBeInRole [Role.Admin; Role.Trener; Role.Skribent] >=> choose [ 
                                    routef "/%O/score/home" (Games.Api.setHomeScore club.Id >> jsonPost)
                                    routef "/%O/score/away" (Games.Api.setAwayScore club.Id >> jsonPost)       
                                    routef "/%O/events" (Games.Events.Api.add club.Id >> jsonPost)       
                                    routef "/%O/events/%O/delete" (Games.Events.Api.delete club.Id >> jsonGet)       

                                ]
                                mustBeInRole [Role.Trener] >=> choose [                                
                                    routef "/%O/squad/select/%O" (Games.Api.selectPlayer club.Id >> jsonPost)     
                                    routef "/%O/gameplan" (Games.Api.setGamePlan club.Id >> jsonPost)
                                    routef "/%O/gameplan/publish" (Games.Api.publishGamePlan club.Id >> jsonPost)
                                ]                                    
                            GET >=> 
                                routef "/%O/squad" (Games.Api.getSquad club.Id >> jsonGet)
                                route "/events/types" >=> (Games.Events.Api.getTypes |> jsonGet)
                                routef "/%O/events" (Games.Events.Api.get club.Id >> jsonGet)                                
                        ])                                                                                                                                                                                                                                     
                    text "404"
                   ] next ctx
            | None ->
                text "404_2" next ctx

    let useGiraffe (app : IApplicationBuilder)  =
            app.UseGiraffe webApp

    let addGiraffe (services : IServiceCollection)  =
        services.AddGiraffe() |> ignore

    let registerJsonSerializers (services : IServiceCollection)  =
        let settings = JsonSerializerSettings ()
        settings.ContractResolver <- Serialization.CamelCasePropertyNamesContractResolver()   
        settings.Converters.Add(OptionConverter())
        settings.Converters.Add(IdiomaticDuConverter())
        settings.Converters.Add(StringEnumConverter())
        services.AddSingleton<IJsonSerializer>(NewtonsoftJsonSerializer(settings)) |> ignore


