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
        let members : Member list = []
        let squad : Member list = []

        let imageOptions = {
            ApiKey = "string"
            ApiSecret = "string"
            CloudName = "string"
            DefaultMember = "string"
            DefaultArticle = "string"
        }

        let listPlayers (players: Member list) = 
            div [ Class "col-sm-6 col-xs-12" ]
               [ div [ Class "collapselink-parent" ]
                   [ a [ Class "collapse-link "
                         Role "button"
                         DataToggle "collapse"
                         Href "#ra-otherplayers-signedup"
                         AriaExpanded true ]
                       [ str <| sprintf "Påmeldte spillere (%i)" players.Length ] ]
                 div [ Id "ra-otherplayers-signedup"
                       Class "collapse in" ]
                   [ 
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
                                        //str "@if (!string.IsNullOrWhiteSpace(player.Attendance?.SignupMessage))" 
                                    // <a class="mt-popover registerSquad-messageIcon" data-container="body" data-content="@player.Attendance.SignupMessage" data-placement="right" data-toggle="popover" href="javascript:void(0);"><i class="fa fa-comment"></i></a>
                                        
                                    // <span id="playerAttendance-@player.Id" title="Oppmøte siste 8 uker" class="register-attendance-attendance">0%</span>
                                    // <input class="form-control register-attendance-input"
                                    //     data-player-id="@player.Id"
                                    //     data-player-name="@player.ShortName"
                                    //     data-event-id="@player.EventId"
                                    //     type="checkbox"
                                    //     checked
                                    //     />
                                    ]
                                  div [ Class "ra-info-elements" ]
                                    [ span [ Class "label label-danger" ]
                                        [ i [ Class "fa fa-exclamation-triangle" ]
                                            [ ] ] ] ]
                        ))
                   ]
                 br [ ]

               ]


        mtMain [] [
            block [Id "registerSquad"] 
                    [
                        editLink <| sprintf "/intern/arrangement/endre/%O" game.Id
                        a [ Href "game/gameplan/gameId"
                            Class "registerSquad-gameplan-link pull-right"
                            Title "Bytteplan" ]
                            [ 
                               Icons.gamePlan
                            ]
                        div [ Class "flex" ]
                           [ div [ Class "flex-1 event-icon align-center" ]
                               [  
                                Icons.eventIcon EventType.Kamp Icons.ExtraLarge       
                               ]
                             div [ Class "flex-2 faded" ]
                               [ p [ ]
                                   [ 
                                    Icons.calendar ""
                                    str " "
                                    game.Date |> Date.format |> str
                                    span [ Class "no-wrap" ]
                                       [ 
                                        Icons.time ""  
                                        str " " 
                                        game.Date |> Date.formatTime |> str
                                        ] 
                                     ]
                                 p [ ]
                                   [ 
                                        Icons.mapMarker ""
                                        str " "
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
                                        ul [ Id "squad"; Class "list-unstyled squad-list" ] 
                                            (
                                                squad 
                                                |> List.map (fun m ->
                                                                li [ Id "@player.Id" ] [ 
                                                                    Icons.player ""
                                                                    str <| sprintf " %s" m.Name]  
                                                )
                                            )
                                     ]
                                 hr [ ]
                                 div [ Class "registerSquad-publish"
                                       Id "registerSquad-publish" ]
                                   [ div [ Class "relative registerSquad-messageWrapper" ]
                                       [ textarea [ Id "publishMessage"
                                                    Class "form-control"
                                                    HTMLAttr.Custom ("data-event-id", "@Model.Game.Id")
                                                    Placeholder "Beskjed til spillerne" ]
                                           [ str "@Model.Game.Description" ]
                                         span [ Class "label-feedback label label-danger" ]
                                           [ i [ Class "fa fa-exclamation-triangle" ]
                                               [ ] ]
                                         span [ Class "label-feedback label label-success" ]
                                           [ i [ Class "fa fa-check" ]
                                               [ ] ] ]
                                     div [ ]
                                       [
                                        (if true then // game.IsPublished then 
                                            btn Success Lg [Class "disabled"]
                                               [ Icons.checkCircle 
                                                 str "Publisert" ]
                                        else
                                            div [] [
                                                btn Primary Lg []
                                                  [ str "Publiser tropp"    
                                                    i [ Class "fa fa-spinner fa-spin" ]
                                                      [ ] ]
                                                span [ Class "label-feedback label label-danger" ]
                                                  [ i [ Class "fa fa-exclamation-triangle" ]
                                                      [ ] ] 
                                            ]
                                        )       
                                        ] 
                                    ] 
                                ] 
                            ]                    
                       ]                                        
        ]

ReactDom.render(element, document.getElementById(ClientViews.selectSquad))

