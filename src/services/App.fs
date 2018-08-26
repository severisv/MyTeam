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
open Microsoft.AspNetCore.Hosting
open PipelineHelpers

    
module App =

    let webApp =
        removeTrailingSlash >=>
        fun next ctx ->
            let (club, user) = Tenant.get ctx
            let mustBeInRole = mustBeInRole user

            match club with
            | Some club ->
                choose [
                    route "/404" >=> setStatusCode 404 >=> Views.Error.notFound club user    
                    route "/" >=> (News.Pages.Index.view club user id |> htmlGet)                         
                    routef "/nyheter/%i/%i" <| fun (skip, take) -> redirectTo true <| sprintf "/%i/%i" skip take                         
                    routef "/%i/%i" <| fun (skip, take) -> News.Pages.Index.view club user (fun o -> { o with Skip = skip; Take = take }) |> htmlGet                        
                    routef "/nyheter/vis/%s" <| fun name -> News.Pages.Show.view club user name |> htmlGet                      
                    routef "/nyheter/endre/%s" <| fun name -> 
                        mustBeInRole [Role.Admin; Role.Trener; Role.Skribent] >=> 
                            choose  [
                                        GET >=> (user => fun user -> (News.Pages.Edit.view club user name |> htmlGet))                   
                                        POST >=> (user => fun user -> (News.Pages.Edit.editPost club user name |> htmlGet))                                            
                                    ]
                    route "/nyheter/ny" >=>
                        mustBeInRole [Role.Admin; Role.Trener; Role.Skribent] >=> 
                            choose  [
                                        GET >=> (user => fun user -> (News.Pages.Edit.create club user |> htmlGet))                   
                                        POST >=> (user => fun user -> (News.Pages.Edit.createPost club user |> htmlGet))                                            
                                    ]
                    routef "/nyheter/slett/%s" <| fun name -> 
                        mustBeInRole [Role.Admin; Role.Trener; Role.Skribent] >=> 
                                        GET >=> (News.Pages.Edit.delete club name |> htmlGet)                   
                                                                                                                                                                                                                                         
                    route "/kamper" >=> (Games.Pages.List.view club user None None |> htmlGet)   
                    routef "/kamper/vis/%O" <| fun gameId -> Games.Pages.Show.view club user gameId |> htmlGet 
                    routef "/kamper/%s/%i" <| fun (teamName, year) -> Games.Pages.List.view club user (Some teamName) (Some year) |> htmlGet         
                    routef "/kamper/%s" <| fun teamName -> Games.Pages.List.view club user (Some teamName) None |> htmlGet 
                    route "/tabell" >=> (Table.Pages.index club user None None |> htmlGet)      
                    routef "/tabell/%s/%s" <| fun (teamName, year) -> Table.Pages.index club user (Some teamName) (Some year) |> htmlGet       
                    routef "/tabell/%s" <| fun teamName -> Table.Pages.index club user (Some teamName) None |> htmlGet        
                    route "/statistikk" >=> (Stats.Pages.index club user None None |> htmlGet)   
                    routef "/statistikk/%s/%s" <| fun (teamName, year) -> Stats.Pages.index club user (Some teamName) (Some year) |> htmlGet         
                    routef "/statistikk/%s" <| fun teamName -> Stats.Pages.index club user (Some teamName) None |> htmlGet      
                    route "/personvern" >=> (AboutPages.privacy club user |> htmlGet)          
                    route "/om" >=> (AboutPages.index club user |> htmlGet)        
                    subRoute "/intern" 
                        mustBeMember >=>
                            (user => fun user ->
                                            choose [                                                
                                                GET >=> choose [    
                                                    route "/lagliste" >=> (Members.Pages.List.view club user None |> htmlGet)
                                                    routef "/lagliste/%s" <| fun status -> Members.Pages.List.view club user (Some status) |> htmlGet
                                                    route "/oppmote/registrer" >=> mustBeInRole [Role.Admin; Role.Trener; Role.Oppmøte] 
                                                        >=> (Attendance.Pages.Register.view club user None |> htmlGet)
                                                    routef "/oppmote/registrer/%O" <| fun eventId -> mustBeInRole [Role.Admin; Role.Trener; Role.Oppmøte] 
                                                                                                    >=> (Attendance.Pages.Register.view club user (Some eventId) |> htmlGet)                                                       
                                                    route "/oppmote" >=> (Attendance.Pages.Show.view club user None |> htmlGet)
                                                    routef "/oppmote/%s" <| fun year -> Attendance.Pages.Show.view club user (Some <| toLower year) |> htmlGet
                                                ]                    
                                            ]
                                        )    
                    route "/admin" >=> mustBeInRole [Role.Admin; Role.Trener]  >=> (Admin.Pages.index club user |> htmlGet)
                    route "/admin/spillerinvitasjon" >=> mustBeInRole [Role.Admin; Role.Trener]  >=> (Admin.Pages.invitePlayers club user |> htmlGet)
                    subRoute "/api/attendance"                            
                           <| choose [ 
                                GET >=> routef "/%O/recent" (Attendance.Api.getRecentAttendance club >> jsonGet)
                                POST >=> mustBeInRole [Role.Admin; Role.Trener; Role.Oppmøte] >=> 
                                    routef "/%O/registrer/%O" (Attendance.Api.confirmAttendance club.Id >> jsonPost)
                            ]                                     
                                                    
                    route "/api/teams" >=> (Teams.Api.list club.Id |> jsonGet)
                    subRoute "/api/events"                    
                        (choose [
                            PUT >=> mustBeInRole [Role.Admin; Role.Trener] >=> 
                                routef "/%O/description" (Events.Api.setDescription club.Id)
                        ])
                    subRoute "/api/members" 
                        <| choose [ 
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
                        ]
                    subRoute "/api/games"
                        <| choose [                                                                
                            GET >=>  choose [
                                routef "/%O/squad" (Games.Api.getSquad >> jsonGet)
                                route "/events/types" >=> (Games.Events.Api.getTypes |> jsonGet)
                                routef "/%O/events" (Games.Events.Api.get club.Id >> jsonGet)      
                            ]     
                            POST >=> 
                                mustBeInRole [Role.Admin; Role.Trener; Role.Skribent] >=> choose [ 
                                    routef "/%O/score/home" (Games.Api.setHomeScore club.Id >> jsonPost)
                                    routef "/%O/score/away" (Games.Api.setAwayScore club.Id >> jsonPost)       
                                    routef "/%O/events" (Games.Events.Api.add club.Id >> jsonPost)       
                                    routef "/%O/events/%O/delete" (Games.Events.Api.delete club.Id >> jsonGet)       
                                    routef "/%O/squad/select/%O" (Games.Api.selectPlayer club.Id >> jsonPost)    
                                ]
                                mustBeInRole [Role.Trener] >=> choose [         
                                    routef "/%O/squad/publish" (Games.Api.publishSquad club.Id >> jsonPost)                           
                                    routef "/%O/gameplan" (Games.Api.setGamePlan club.Id >> jsonPost)
                                    routef "/%O/gameplan/publish" (Games.Api.publishGamePlan club.Id >> jsonPost)
                                ]                     
                        ]                                                                                                                                                                                                                                     
                    setStatusCode 404 >=> ErrorHandling.logNotFound >=> Views.Error.notFound club user
                   ] next ctx
            | None ->
                (setStatusCode 404 >=> ErrorHandling.logNotFound >=> text "404") next ctx

            
    let useGiraffe (app : IApplicationBuilder)  =
            let env = app.ApplicationServices.GetService<IHostingEnvironment>()
            if env.IsDevelopment() then
                app.UseDeveloperExceptionPage() |> ignore
                app.UseGiraffe webApp
            else 
                app.UseGiraffeErrorHandler(ErrorHandling.errorHandler)
                   .UseGiraffe webApp

    let addGiraffe (services : IServiceCollection)  =
        services.AddGiraffe() |> ignore

    let registerJsonSerializers (services : IServiceCollection)  =
        let settings = JsonSerializerSettings ()
        settings.ContractResolver <- Serialization.CamelCasePropertyNamesContractResolver()   
        settings.Converters.Add(OptionConverter())
        settings.Converters.Add(IdiomaticDuConverter())
        settings.Converters.Add(StringEnumConverter())
        services.AddSingleton<IJsonSerializer>(NewtonsoftJsonSerializer(settings)) |> ignore


