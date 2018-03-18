namespace MyTeam

open Giraffe
open Giraffe.Serialization
open Giraffe.GiraffeViewEngine
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open MyTeam
open MyTeam.Domain.Members
open MyTeam.Authorization
open MyTeam.Views
open MyTeam.Models.Enums
open System
open Newtonsoft.Json
open Newtonsoft.Json.Converters

    
type Asd = {
    Option: string option
    Tuple: int*string
}

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
                                                    GET >=> mustBeInRole [Role.Admin; Role.Trener; Role.Oppmøte] >=>  choose [ 
                                                        route "/oppmote/registrer" >=> AttendancePages.Register.view club user None
                                                        routef "/oppmote/registrer/%O" (fun eventId -> AttendancePages.Register.view club user (Some eventId))                                                       
                                                    ]
                                                    GET >=> choose [                                                        
                                                        route "/oppmote" >=> AttendancePages.Show.view club user None
                                                        routef "/oppmote/%s" (fun year -> AttendancePages.Show.view club user (Some <| toLower year))
                                                    ]                                    
                                                ])
                                        )                        
                                        empty
                            )
                        subRoute "/api/attendance"                            
                            (choose [ 
                                GET >=> routef "/%O/recent" (fun teamId -> AttendanceApi.getRecentAttendance club teamId)
                                POST >=> mustBeInRole [Role.Admin; Role.Trener; Role.Oppmøte] >=> 
                                    routef "/%O/registrer/%O" (fun (eventId, playerId) ->
                                        AttendanceApi.confirmAttendance club eventId playerId)                                   
                                                               
                            ])                                      
                                                                                                          
                        
                        route "/api/teams" >-> TeamApi.list club.Id
                        route "/api/events" >=>                      
                            PUT >=> mustBeInRole [Role.Admin; Role.Trener] >=> 
                                routef "/api/events/%O/description" (EventApi.setDescription club.Id)
                        
                        subRoute "/api/members" 
                            (choose [ 
                                GET >=> choose [ 
                                    route "" >-> MemberApi.list club.Id
                                    route "/facebookids" >-> MemberApi.getFacebookIds club.Id
                                ]
                                PUT >=> 
                                    mustBeInRole [Role.Admin; Role.Trener] >=> 
                                        choose [ 
                                            routef "/%O/status" (MemberApi.setStatus club.Id)
                                            routef "/%O/togglerole" (MemberApi.toggleRole club.Id)
                                            routef "/%O/toggleteam" (MemberApi.toggleTeam club.Id)
                                        ]       
                                POST >=>  
                                    mustBeInRole [Role.Admin; Role.Trener] >=> 
                                        choose [ 
                                            route "" >=> MemberApi.add club.Id
                                        ]
                            ])

                        subRoute "/api/games"
                            (choose [
                                GET >=> 
                                    routef "/%O/squad" (GameApi.getSquad club.Id)
                                                           
                                POST >=> 
                                    mustBeInRole [Role.Admin; Role.Trener; Role.Skribent] >=> choose [ 
                                        routef "/%O/score/home" (GameApi.setHomeScore club.Id)
                                        routef "/%O/score/away" (GameApi.setAwayScore club.Id)                                   
                                    ]
                                    mustBeInRole [Role.Trener] >=> choose [                                
                                        routef "/%O/gameplan" (GameApi.setGamePlan club.Id)
                                        routef "/%O/gameplan/publish" (GameApi.publishGamePlan club.Id)
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

    let registerJsonSerializers (services : IServiceCollection)  =
        let settings = JsonSerializerSettings ()
        settings.ContractResolver <- Serialization.CamelCasePropertyNamesContractResolver()   
        settings.Converters.Add(OptionConverter())
        settings.Converters.Add(IdiomaticDuConverter())
        services.AddSingleton<IJsonSerializer>(NewtonsoftJsonSerializer(settings)) |> ignore


