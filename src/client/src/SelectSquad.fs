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


let element = 

        let game : Event = {
            Id = Guid.NewGuid()
            Location = "Muselunden"
            Date = DateTime.Now
        }
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
        let squad = members

        let imageOptions = {
            ApiKey = "string"
            ApiSecret = "string"
            CloudName = "string"
            DefaultMember = "string"
            DefaultArticle = "string"
        }

        let isPublished = true
        let description = "Beskrivelse"
        let signupMessage = "kommer ikke"

        let listPlayers (players: Member list) = 
            div [ Class "col-sm-6 col-xs-12" ]
               [ 
                 collapsible Open [
                     str <| sprintf "Påmeldte spillere (%i)" players.Length
                 ] [     
                    ul [ Class "list-users" ]
                        (squad
                        |> List.map(fun m ->
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
                                    Strings.hasValue signupMessage &?
                                        tooltip signupMessage [Class "registerSquad-messageIcon"] [
                                            Icons.comment
                                        ]
                                                                            
                                    span [
                                            Id <| sprintf "playerAttendance-%O" m.Id
                                            Title "Oppmøte siste 8 uker"
                                            Class "register-attendance-attendance"] 
                                        [str "0%"]
                                    input [
                                        Class "form-control register-attendance-input"
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
                             listPlayers members
                             div [ Class "col-sm-6 col-xs-12 " ]
                               [ h2 [ ] [ str <| sprintf "Tropp (%i)" squad.Length ]   
                                 hr [ ]
                                 div [ ]
                                     [ 
                                        ul [ Class "list-unstyled squad-list" ] 
                                            (
                                                squad 
                                                |> List.map (fun m ->
                                                                li [] [ 
                                                                    Icons.player ""
                                                                    str <| sprintf " %s" m.Name]  
                                                )
                                            )
                                     ]
                                 hr [ ]
                                 div [ Class "registerSquad-publish" ]
                                   [ div [ Class "relative registerSquad-messageWrapper" ]
                                       [ textarea [ Class "form-control"
                                                    Placeholder "Beskjed til spillerne"
                                                    Value description
                                                    OnChange (fun o -> printf "%O"  o)
                                                 ]
                                           [ ]
                                         Labels.error
                                         Labels.success 
                                       ]
                                     div [ ]
                                       [
                                        (if isPublished then 
                                            btn Success Lg [Class "disabled"]
                                               [ Icons.checkCircle 
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

