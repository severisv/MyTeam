module MyTeam.Client.SelectSquad

open MyTeam.Domain.Members
open System
open Fable.Helpers.React
open Fable.Import
open Fable.Import.Browser
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Import.React
open MyTeam.Domain
open MyTeam.Domain.Events
open MyTeam.Shared
open MyTeam.Shared.Components
open MyTeam.Shared.Components.Layout
open MyTeam.Components
open MyTeam

type Signup = {
        MemberId: Guid
        IsAttending: bool
        Message: string        
    }

type Squad = {
    MemberIds: Guid list
    IsPublished: bool
}

type RecentAttendance = {
    MemberId: Guid
    AttendancePercentage: int
}

type GameDetailed = {
    Id: Guid
    Date: DateTime
    Location: string
    Description: string
    Squad: Squad
}

type Player = Member * Signup option

let element = 
    
        let recentAttendance : RecentAttendance list = []

        let getRecentAttendance memberId = 
            recentAttendance
            |> List.tryFind (fun a -> a.MemberId = memberId)
            |> function
            | Some a -> sprintf "%i%%" a.AttendancePercentage
            | None -> ""

        let game : GameDetailed = {
            Id = Guid.NewGuid()
            Location = "Muselunden"
            Date = DateTime.Now
            Description = "Oppmøte 19.30"
            Squad = {
                    IsPublished = true
                    MemberIds = []
                }
        }

       

        let players: Player list = 
            let members : Member list = [
                { 
                    Id = Guid.NewGuid()
                    FacebookId = "string"
                    FirstName = "string"
                    MiddleName = "string"
                    LastName = "string"
                    UrlName = "string"
                    Image = "string"    
                    Status = Status.Aktiv      
                }
            ]

            let signups: Signup list = []

            members
            |> List.map (fun m ->
                let s = signups 
                        |> List.tryFind (fun s -> s.MemberId = m.Id)                                 
                (m, s)
            )

        let imageOptions = {
            ApiKey = "string"
            ApiSecret = "string"
            CloudName = "string"
            DefaultMember = "string"
            DefaultArticle = "string"
        }

    

        let listPlayers (players: Player list) = 
            div [ Class "col-sm-6 col-xs-12" ]
               [ 
                 collapsible Open [
                     str <| sprintf "Påmeldte spillere (%i)" players.Length
                 ] [     
                    ul [ Class "list-users" ]
                        (players
                        |> List.map(fun (m, s) ->
                            li [ Class "register-attendance-item registerSquad-player" ]
                                [ span [ ]
                                    [ 
                                        img [ Class "hidden-xxs"
                                              Src <| MyTeam.Image.getMember imageOptions m.Image m.FacebookId 
                                                        (fun opts -> { opts with Height = Some 50; Width = Some 50 })
                                           ]
                                        str m.Name 
                                        ]
                                  span [ ]
                                    [ 
                                    s => fun s -> 
                                        Strings.hasValue s.Message &?
                                            tooltip s.Message [Class "registerSquad-messageIcon"] [
                                                Icons.comment
                                            ]
                                                                            
                                    span [
                                            Id <| sprintf "playerAttendance-%O" m.Id
                                            Title "Oppmøte siste 8 uker"
                                         ] 
                                        [str <| getRecentAttendance m.Id]
                                    input [
                                        Class "form-control"
                                        Type "checkbox"
                                        Checked true
                                    ]                                    
                                    ]
                                  div [ Class "ra-info-elements" ]
                                         [ Labels.error ] 
                                ]
                        ))
                   ]
                 br [ ]

               ]

        let squad = players 
                    |> List.filter (fun (m, _) -> game.Squad.MemberIds |> List.contains m.Id)

        mtMain [] [
            block [] 
                    [
                        editLink <| sprintf "/intern/arrangement/endre/%O" game.Id
                        a [ Href "game/gameplan/gameId"
                            Class "registerSquad-gameplan-link pull-right"
                            Title "Bytteplan" ]
                            [ 
                               Icons.gamePlan
                            ]
                        div [ Class "rs-header flex" ]
                           [ div [ Class "flex-1 event-icon align-center flex-center" ]
                               [  
                                Icons.eventIcon EventType.Kamp Icons.ExtraLarge       
                               ]
                             div [ Class "flex-2 faded" ]
                               [ p [ ]
                                   [ 
                                    span [ ] [ 
                                         Icons.calendar ""
                                         whitespace
                                         game.Date |> Date.format |> str ]
                                    whitespace
                                    span [ Class "no-wrap" ]
                                       [ 
                                        whitespace
                                        Icons.time ""  
                                        whitespace
                                        game.Date |> Date.formatTime |> str
                                        ] 

                                     ]
                                 p [ ]
                                   [ 
                                        Icons.mapMarker ""
                                        whitespace
                                        str game.Location 
                                    ] 
                                ] 
                            ]
                        div [ Class "row" ]
                           [
                             listPlayers players
                             div [ Class "col-sm-6 col-xs-12 " ]
                               [ h2 [ ] [ str <| sprintf "Tropp (%i)" squad.Length ]   
                                 hr [ ]
                                 div [ ]
                                     [ 
                                        ul [ Class "list-unstyled squad-list" ] 
                                            (
                                                squad 
                                                |> List.map (fun (m, s) ->
                                                                li [] [ 
                                                                    Icons.player ""
                                                                    str <| sprintf " %s" m.Name]  
                                                )
                                            )
                                     ]
                                 hr [ ]
                                 div [ Class "registerSquad-publish" ]
                                   [ div [ Class "registerSquad-messageWrapper" ]
                                       [ textarea [ Class "form-control"
                                                    Placeholder "Beskjed til spillerne"
                                                    Value game.Description
                                                    OnChange (fun o -> printf "%O"  o)
                                                 ]
                                           [ ]
                                         Labels.error
                                         Labels.success 
                                       ]
                                     div [ ]
                                       [
                                        (if game.Squad.IsPublished then 
                                            btn Success Lg [Class "disabled"]
                                               [ Icons.checkCircle 
                                                 whitespace
                                                 str "Publisert" ]
                                        else
                                            div [] [
                                                btn Primary Lg []
                                                  [ str "Publiser tropp"    
                                                    Icons.spinner
                                                  ]
                                                Labels.error
                                            ]
                                        )       
                                        ] 
                                    ] 
                                ] 
                            ]                    
                       ]                                        
        ]
        
ReactDom.render(element, document.getElementById(ClientViews.selectSquad))

