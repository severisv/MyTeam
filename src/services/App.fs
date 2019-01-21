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
                    
                    route "/" >=> GET >=> (News.Pages.Index.view club user id |> htmlGet)   
                    routef "/%i/%i" <| fun (skip, take) -> GET >=> (News.Pages.Index.view club user (fun o -> { o with Skip = skip; Take = take }) |> htmlGet)                        

                    subRoute "/nyheter"             
                        <|  choose [                                                
                                    routef "/%i/%i" <| fun (skip, take) -> GET >=> (redirectTo true <| sprintf "/%i/%i" skip take)                         
                                    routef "/vis/%s" <| fun name -> GET >=> (News.Pages.Show.view club user name |> htmlGet)                      
                                    routef "/endre/%s" <| fun name -> 
                                        mustBeInRole [Role.Admin; Role.Trener; Role.Skribent] >=> 
                                            choose  [
                                                        GET >=> (user => fun user -> (News.Pages.Edit.view club user name |> htmlGet))                   
                                                        POST >=> (user => fun user -> (News.Pages.Edit.editPost club user name |> htmlGet))                                            
                                                    ]
                                    route "/ny" >=>
                                        mustBeInRole [Role.Admin; Role.Trener; Role.Skribent] >=> 
                                            choose  [
                                                        GET >=> (user => fun user -> (News.Pages.Edit.create club user |> htmlGet))                   
                                                        POST >=> (user => fun user -> (News.Pages.Edit.createPost club user |> htmlGet))                                            
                                                    ]
                                    routef "/slett/%s" <| fun name -> 
                                        mustBeInRole [Role.Admin; Role.Trener; Role.Skribent] >=> 
                                                        GET >=> (News.Pages.Edit.delete club name |> htmlGet)       
                            ] 
                            
                    subRoute "/kamper"             
                        <|  choose [                                                
                                GET >=> choose [    
                                    route "" >=> (Games.Pages.List.view club user None None |> htmlGet)
                                    routef "/vis/%O" <| fun gameId -> Games.Pages.Show.view club user gameId |> htmlGet 
                                    routef "/%O/resultat" <| fun gameId -> 
                                        mustBeInRole [Role.Admin; Role.Trener; Role.Skribent] >=> (Games.Pages.Result.view club user gameId |> htmlGet) 
                                    routef "/%O/laguttak" <| fun gameId -> 
                                        mustBeInRole [Role.Trener] >=> (Games.Pages.SelectSquad.view club user gameId |> htmlGet) 
                                    routef "/%O/bytteplan" <| fun gameId -> 
                                        user => fun user -> (Games.Pages.GamePlan.view club user gameId |> htmlGet) 
                                    routef "/%s/%i" <| fun (teamName, year) -> Games.Pages.List.view club user (Some teamName) (Some year) |> htmlGet         
                                    routef "/%s" <| fun teamName -> Games.Pages.List.view club user (Some teamName) None |> htmlGet 
                                ]
                            ]                                                                                                                                                                                                                       
                    subRoute "/tabell"   
                        <| choose [
                            GET >=> choose [
                                        route "" >=> (Table.Table.view club user None None |> htmlGet)      
                                        routef "/%s/%s" <| fun (teamName, year) -> Table.Table.view club user (Some teamName) (Some year) |> htmlGet       
                                        routef "/%s" <| fun teamName -> Table.Table.view club user (Some teamName) None |> htmlGet        
                            ]
                        ]          
                    
                    subRoute "/statistikk"   
                        <| choose [
                             GET >=> choose [
                                               route "" >=> (Stats.Pages.index club user None None |> htmlGet)   
                                               routef "/%s/%s" <| fun (teamName, year) -> Stats.Pages.index club user (Some teamName) (Some year) |> htmlGet         
                                               routef "/%s" <| fun teamName -> Stats.Pages.index club user (Some teamName) None |> htmlGet      
                                ]                        
                        ]                
                
                    route "/om" >=> GET >=> (About.show club user |> htmlGet)        
                    route "/om/endre" >=> 
                        mustBeInRole [Role.Admin] >=> 
                            choose  [
                                        GET >=> (About.edit club user |> htmlGet)                   
                                        POST >=> (About.editPost club user |> htmlGet)                   
                                    ]                    
                    route "/støttespillere" >=> GET >=> (Sponsors.show club user |> htmlGet)        
                    route "/støttespillere/endre" >=> 
                        mustBeInRole [Role.Admin] >=> 
                            choose  [
                                        GET >=> (Sponsors.edit club user |> htmlGet)                   
                                        POST >=> (Sponsors.editPost club user |> htmlGet)                   
                                    ]
                    
                       
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
                    route "/admin" >=> GET >=> mustBeInRole [Role.Admin; Role.Trener]  >=> (Admin.Pages.index club user |> htmlGet)
                    route "/admin/spillerinvitasjon" >=> GET >=> mustBeInRole [Role.Admin; Role.Trener]  >=> (Admin.Pages.invitePlayers club user |> htmlGet)
                    
                    subRoute "/api/attendance"                            
                           <| choose [ 
                                POST >=> mustBeInRole [Role.Admin; Role.Trener; Role.Oppmøte] >=> 
                                    routef "/%O/registrer/%O" (Attendance.Api.confirmAttendance club.Id >> jsonPost)
                            ]                                     
                                                    
                    route "/api/teams" >=> (Teams.Api.list club.Id |> jsonGet)
                    subRoute "/api/events"                    
                        (choose [
                            PUT >=> mustBeInRole [Role.Admin; Role.Trener] >=> 
                                routef "/%O/description" (Events.Api.setDescription club.Id >> jsonPost)
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
                                routef "/insights/%s/%i" (Games.Api.getInsights club >> jsonGet)
                                routef "/%O/squad" (Games.Api.getSquad >> jsonGet)
                                route "/events/types" >=> (Gameevents.getTypes |> jsonGet)
                                routef "/%O/events" (Gameevents.get club.Id >> jsonGet)      
                                route "/refresh" >=> Games.Refresh.run

                            ]     
                            POST >=> 
                                mustBeInRole [Role.Admin; Role.Trener; Role.Skribent] >=> choose [ 
                                    routef "/%O/score/home" (Games.Api.setHomeScore club.Id >> jsonPost)
                                    routef "/%O/score/away" (Games.Api.setAwayScore club.Id >> jsonPost)       
                                    routef "/%O/events" (Gameevents.add club.Id >> jsonPost)       
                                    routef "/%O/events/%O/delete" (Gameevents.delete club.Id >> jsonGet)       
                                    routef "/%O/squad/select/%O" (Games.Api.selectPlayer club.Id >> jsonPost)    
                                ]
                                mustBeInRole [Role.Trener] >=> choose [         
                                    routef "/%O/squad/publish" (Games.Api.publishSquad club.Id >> jsonPost)                           
                                    routef "/%O/gameplan" (Games.Api.setGamePlan club.Id)
                                    routef "/%O/gameplan/publish" (Games.Api.publishGamePlan club.Id >> jsonPost)
                                ]                     
                        ]
                    subRoute "/api/tables"
                        <| choose [                                                                
                            GET >=>  choose [
                                route "/refresh" >=> Table.Refresh.run
                            ]
                            mustBeInRole [Role.Admin] >=> 
                                choose [
                                    PUT >=> choose [
                                        routef "/%s/%i/title" (Table.Api.setTitle club >> jsonPost)
                                        routef "/%s/%i/fixturesourceurl" (Table.Api.setFixtureSourceUrl club >> jsonPost)
                                        routef "/%s/%i/sourceurl" (Table.Api.setSourceUrl club >> jsonPost)
                                    ]
                                    POST >=> choose [
                                        routef "/%s/%i/autoupdate" (Table.Api.setAutoUpdate club >> jsonPost)
                                        routef "/%s/%i/autoupdatefixtures" (Table.Api.setAutoUpdateFixtures club >> jsonPost)
                                        routef "/%s/%i" (Table.Api.create club >> jsonPost)
                                    ]                             
                                    DELETE >=> 
                                        routef "/%s/%i" (Table.Api.delete club >> jsonGet)
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


