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
open System


module Asdf = 

    type Training = {
        Id: Guid
        Date: DateTime       
        Location: string
    }


    type PlayerAttendance = {
        Id: Guid
        FacebookId: string
        FirstName: string
        LastName: string
        UrlName: string
        Image: string       
        DidAttend: bool       
    } with 
        member p.Name = sprintf "%s %s" p.FirstName p.LastName        

    let index club user trainingId next (ctx: HttpContext) =


        let db = ctx.Database
        
        let getImage = Images.getMember ctx

        let selectedTraining = 
            {
                Id = Guid.NewGuid()
                Date = DateTime.Now
                Location = "Kringsjå"
            }
    

        let registerAttendanceUrl trainingId = 
            sprintf "/intern/oppmote/registrer/%s" (str trainingId)     

        let isSelected url = 
            registerAttendanceUrl selectedTraining.Id = url      

        let previousTrainings = 
            [selectedTraining]        



        let editEventLink eventId =
            editLink <| sprintf "/intern/arrangement/endre/%s" (str eventId)          
            

        let attendees = 
            [
                {
                    Id = (Guid.NewGuid())
                    FirstName = "Severin"
                    LastName = "Sverdvik"
                    FacebookId = ""
                    Image = ""
                    UrlName = "severin_sverdvik"
                    DidAttend = true
                }
            ]        
   
        
             

        let registerAttendancePlayerList header (players: PlayerAttendance list) =
            collapsible 
                false 
                [encodedText <| sprintf "%s (%i)" header players.Length]
                [
                    ul [ _class "list-users col-xs-11 col-md-10" ] 
                        (players |> List.map (fun p ->
                            li [ _class "register-attendance-item" ] [ 
                                img [_src <| getImage p.Image p.FacebookId (fun o -> { o with Height = Some 50; Width = Some 50})]
                                encodedText p.Name
                                input [ 
                                    _class "pull-right form-control register-attendance-input"
                                    attr "data-player-id" (str p.Id)
                                    attr "data-event-id" (str selectedTraining.Id)
                                    _type "checkbox" 
                                    (p.DidAttend =? (_checked, _empty))
                                    ]
                                ajaxSuccessIndicator
                            ]
                        ))             
                ]                                    

        ([
            main [_class "register-attendance"] [
                block [] [                  
                    editEventLink selectedTraining.Id
                    div [_class "attendance-event" ] [
                        eventIcon EventType.Trening ExtraLarge        
                        div [ _class "faded" ] [ 
                            p [] [
                                icon (fa "calendar") "" 
                                whitespace
                                encodedText <| (selectedTraining.Date.ToString("ddd d MMMM"))                     
                            ]                     
                            p [] [ 
                                    icon (fa "map-marker") ""
                                    encodedText selectedTraining.Location
                     
                                ]
                     
                           ] 
                    ]                    
                    registerAttendancePlayerList "Påmeldte spillere" attendees
                ]  
                     
            ]          
            sidebar [_class "register-attendance"] [               
                (previousTrainings.Length > 0 =?
                    (block [] [
                        navList ({ 
                                    Header = "Siste treninger"
                                    Items = previousTrainings |> List.map (fun training  -> { Text = [icon (fa "calendar") "";whitespace;encodedText <| (training.Date.ToString("ddd d MMMM"))]; Url = registerAttendanceUrl training.Id })  
                                    Footer = None                                                               
                                    IsSelected = isSelected                                                              
                               })
                    ]
                   , emptyText)) 
            ]                                   
                    
        ] 
        |> layout 
            club 
            (Some user) 
            (fun o -> 
                { o with 
                    Title = "Registrer oppmøte"
                    Scripts = [script [_src "/compiled/scripts/event/registerAttendance.js"] []]}
            ) ctx
        |> htmlView) next ctx


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
                                                        route "/oppmote/registrer" >=> Asdf.index club user None
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


