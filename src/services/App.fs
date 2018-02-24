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


module AsdfQueries = 
    type EventId = Guid

    type Training = {
        Id: EventId
        Date: DateTime       
        Location: string
    }

    type Player = {
        Id: Guid
        FacebookId: string
        FirstName: string
        LastName: string
        Image: string       
        DidAttend: bool    
        Status: PlayerStatus   
    } with 
        member p.Name = sprintf "%s %s" p.FirstName p.LastName        



    type Players = {
        Attending: Player list
        OthersActive: Player list
        OthersInactive: Player list
    }    

    type Model = {
        Training: Training
        Players: Players
    }

    type GetPreviousTrainings = Database -> ClubId -> Training list
    type GetTraining = Database -> EventId -> Training
    type GetPlayers = Database -> ClubId -> EventId -> Players

    let getPreviousTrainings: GetPreviousTrainings =
        fun db clubId ->
            let (ClubId clubId) = clubId
            let now = DateTime.Now
            let trening = int EventType.Trening
            query {
                for event in db.Events do
                where (event.ClubId = clubId && event.DateTime < now && event.Type = trening)
                sortByDescending event.DateTime
                take 15
                select 
                    ({
                        Id = event.Id
                        Date = event.DateTime
                        Location = event.Location                    
                    })
            }
            |> Seq.toList         


    let getTraining: GetTraining =
        fun db eventId ->
            query {
                for event in db.Events do
                where (event.Id = eventId)
                select 
                    ({
                        Id = eventId
                        Date = event.DateTime
                        Location = event.Location                    
                    })
            }
            |> Seq.head
                 
    let getPlayers: GetPlayers =
        fun db clubId eventId ->
            let (ClubId clubId) = clubId
            let players =
                let sluttet = int PlayerStatus.Sluttet
                let trener = int PlayerStatus.Trener
                query {
                    for p in db.Players do
                    where (p.ClubId = clubId && p.Status <> sluttet && p.Status <> trener)
                    select (p.Id, p.FirstName, p.LastName, p.FacebookId, p.ImageFull, p.Status)
                } 
                |> Seq.toList

            let attendees = 
                query {
                    for a in db.EventAttendances do
                    where (a.EventId = eventId)
                    select (a.MemberId, a.DidAttend, a.IsAttending)
                } |> Seq.toList
                
            let players = 
                players
                |> List.map (fun (id, firstName, lastName, facebookId, image, status) ->
                    {
                        Id = id
                        FirstName = firstName
                        LastName = lastName
                        FacebookId = facebookId
                        Image = image
                        DidAttend = attendees |> List.exists (fun (playerId, didAttend, _) -> id = playerId && didAttend)
                        Status = enum<PlayerStatus> status
                    }
                 )     
                |> List.sortBy (fun p -> p.Name)
       

            let playerIsAttending p =
                attendees |> List.exists (fun (id, _, isAttending) -> p.Id = id && (isAttending = Nullable true))

            let playerIsActive p =
                p.Status = PlayerStatus.Aktiv    

            let attendingPlayers = players |> List.filter (playerIsAttending)
            let othersActive = players |> List.filter playerIsActive |> List.except attendingPlayers
            let othersInactive = players |> List.except attendingPlayers |> List.except othersActive

            {
                Attending = attendingPlayers
                OthersActive = othersActive
                OthersInactive = othersInactive
            }

open AsdfQueries

module Asdf = 

    let index (club: Club) user trainingId next (ctx: HttpContext) =

        let db = ctx.Database
        
        let previousTrainings = 
            getPreviousTrainings db club.Id
                       
        let getTraining = 
            AsdfQueries.getTraining db
                    
        let model = 
            let getModel (training: Training) = 
                let players = AsdfQueries.getPlayers db club.Id training.Id
                {
                    Training = training
                    Players = players
                }
            match trainingId with
            | Some trainingId -> getTraining trainingId |> getModel |> Some               
            | None -> previousTrainings 
                      |> List.tryHead
                      |> Option.map getModel

                     
        let registerAttendanceUrl (training: Training option) =
            match training with
            | Some training -> sprintf "/intern/oppmote/registrer/%s" (str training.Id) 
            | None -> "/intern/oppmote/registrer"   

        let isSelected url = 
            registerAttendanceUrl (model |> Option.map (fun t -> t.Training)) = url      
 
        let getImage = Images.getMember ctx

        let editEventLink eventId =
            editLink <| sprintf "/intern/arrangement/endre/%s" (str eventId)          
            

        let registerAttendancePlayerList header (players: Player list) (selectedEvent: Training) isCollapsed =
            collapsible 
                isCollapsed 
                [encodedText <| sprintf "%s (%i)" header players.Length]
                [
                    div [_class "row"] [
                        ul [ _class "list-users col-xs-11 col-md-10" ] 
                            (players |> List.map (fun p ->
                                li [ _class "register-attendance-item" ] [ 
                                    img [_src <| getImage p.Image p.FacebookId (fun o -> { o with Height = Some 50; Width = Some 50})]
                                    encodedText p.Name
                                    input [ 
                                        _class "pull-right form-control register-attendance-input"
                                        attr "data-player-id" (str p.Id)
                                        attr "data-event-id" (str selectedEvent.Id)
                                        _type "checkbox" 
                                        (p.DidAttend =? (_checked, _empty))
                                        ]
                                    ajaxSuccessIndicator
                                ]
                            ))  
                        ]                       
                ]                                    


        ([
            main [_class "register-attendance"] [
                block [] 
                        (model
                        |> Option.fold (fun _ model ->                  
                            [
                                editEventLink model.Training.Id
                                div [_class "attendance-event" ] [
                                    eventIcon EventType.Trening ExtraLarge        
                                    div [ _class "faded" ] [ 
                                        p [] [
                                            icon (fa "calendar") "" 
                                            whitespace
                                            encodedText <| (model.Training.Date.ToString("ddd d MMMM"))                     
                                        ]                     
                                        p [] [ 
                                                icon (fa "map-marker") ""
                                                encodedText model.Training.Location
                                 
                                            ]
                                 
                                       ] 
                                ]                    
                                registerAttendancePlayerList "Påmeldte spillere" model.Players.Attending model.Training Open
                                br []
                                registerAttendancePlayerList "Øvrige aktive" model.Players.OthersActive model.Training Collapsed
                                br []
                                registerAttendancePlayerList "Øvrige inaktive" model.Players.OthersInactive model.Training Collapsed
                            ]) 
                            [emptyText]
                            )
                
                     
            ]          
            sidebar [_class "register-attendance"] [               
                (previousTrainings.Length > 0 =?
                    (block [] [
                        navList ({ 
                                    Header = "Siste treninger"
                                    Items = previousTrainings 
                                            |> List.map (fun training  -> 
                                                            { 
                                                                Text = [icon (fa "calendar") "";whitespace;encodedText <| (training.Date.ToString("ddd d MMMM"))];
                                                                Url = registerAttendanceUrl (Some training) 
                                                            }
                                                        )  
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


