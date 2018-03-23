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
                                                        route "/lagliste" >=> Members.Pages.List.view club user None
                                                        routef "/lagliste/%s" (fun status -> Members.Pages.List.view club user (Some status))
                                                    ]                                    
                                                ])
                                        )                        
                                        empty
                            )
                        subRoute "/api/attendance"                            
                            (choose [ 
                                GET >=> routef "/%O/recent" (AttendanceApi.getRecentAttendance club >> jsonGet)
                                POST >=> mustBeInRole [Role.Admin; Role.Trener; Role.Oppmøte] >=> 
                                    routef "/%O/registrer/%O" (AttendanceApi.confirmAttendance club.Id >> jsonPost)
                                                               
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
                                            routef "/%O/toggleteam" (MemberApi.toggleTeam club.Id >> jsonPost)
                                        ]       
                                POST >=>  
                                    mustBeInRole [Role.Admin; Role.Trener] >=> 
                                        choose [ 
                                            route "" >=> (MemberApi.add club.Id |> jsonPost)
                                        ]
                            ])



                        subRoute "/api/games"
                            (choose [
                                POST >=> 
                                    mustBeInRole [Role.Admin; Role.Trener; Role.Skribent] >=> choose [ 
                                        routef "/%O/score/home" (GameApi.setHomeScore club.Id >> jsonPost)
                                        routef "/%O/score/away" (GameApi.setAwayScore club.Id >> jsonPost)       
                                        routef "/%O/events" (GameEventApi.add club.Id >> jsonPost)       
                                        routef "/%O/events/%O/delete" (GameEventApi.delete club.Id >> jsonGet)       

                                    ]
                                    mustBeInRole [Role.Trener] >=> choose [                                
                                        routef "/%O/squad/select/%O" (GameApi.selectPlayer club.Id >> jsonPost)     
                                        routef "/%O/gameplan" (GameApi.setGamePlan club.Id >> jsonPost)
                                        routef "/%O/gameplan/publish" (GameApi.publishGamePlan club.Id >> jsonPost)
                                    ]
                                    
                                GET >=> 
                                    routef "/%O/squad" (GameApi.getSquad club.Id >> jsonGet)
                                    route "/events/types" >=> (GameEventApi.getTypes |> jsonGet)
                                    routef "/%O/events" (GameEventApi.get club.Id >> jsonGet)                                
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
        settings.Converters.Add(StringEnumConverter())
        services.AddSingleton<IJsonSerializer>(NewtonsoftJsonSerializer(settings)) |> ignore


