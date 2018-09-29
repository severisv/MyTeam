module MyTeam.Client.SelectSquad

open MyTeam.Shared.Domain

open Fable.Helpers.React
open Fable.Import
open Fable.Import.Browser
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Import.React
open MyTeam.Shared
open MyTeam.Shared.Components
open MyTeam.Shared.Components.Layout



let element = 
        mtMain [] [
            block [Id "registerSquad"] 
                    [
                        a [ Href "event/edit/gameId"
                            Class "edit-link  pull-right"
                            Title "Rediger kamp" ]
                           [ i [ Class "fa fa-edit" ]
                               [ ] ]
                        a [ Href "game/gameplan/gameId"
                            Class "registerSquad-gameplan-link pull-right"
                            Title "Bytteplan" ]
                           [ i [ Class "fa fa-exchange" ]
                               [ ] ]
                        div [ Class "flex" ]
                           [ div [ Class "flex-1 event-icon align-center" ]
                               [ div [ Class "fa-3x" ]
                                   [ span [ ]
                                       [ str "eventIcon" ] ] ]
                             div [ Class "flex-2 faded" ]
                               [ p [ ]
                                   [ 
                                    Icons.calendar ""
                                    str "Model.Game.DateTime.ToString(ddd d MMMM) &nbsp;"
                                    span [ Class "no-wrap" ]
                                       [ 
                                        Icons.time ""   
                                        str "Model.Game.DateTime.ToString(HH:mm)" ] 
                                     ]
                                 p [ ]
                                   [ 
                                        Icons.mapMarker ""
                                        str "Game.Location" 
                                    ] 
                                ] 
                            ]
                        div [ Class "row" ]
                           [ div [ Class "col-sm-6 col-xs-12" ]
                               [ div [ Class "collapselink-parent" ]
                                   [ a [ Class "collapse-link "
                                         Role "button"
                                         DataToggle "collapse"
                                         Href "#ra-otherplayers-signedup"
                                         AriaExpanded true ]
                                       [ str "Påmeldte spillere (@Model.Attendees.Count())" ] ]
                                 div [ Id "ra-otherplayers-signedup"
                                       Class "collapse in" ]
                                   [ str "@Html.Partial(_RegisterSquadListPlayers, Model.Attendees)" ]
                                 br [ ]
                                 div [ Class "collapselink-parent" ]
                                   [ a [ Class "collapse-link collapsed"
                                         Role "button"
                                         DataToggle "collapse"
                                         Href "#ra-declinees"
                                         AriaExpanded true ]
                                       [ str "Kan ikke (@Model.Declinees.Count())" ] ]
                                 div [ Id "ra-declinees"
                                       Class "collapse" ]
                                   [ str "@Html.Partial(_RegisterSquadListPlayers, Model.Declinees)" ]
                                 br [ ]
                                 div [ Class "collapselink-parent" ]
                                   [ a [ Class "collapse-link collapsed"
                                         Role "button"
                                         DataToggle "collapse"
                                         Href "#ra-otherplayers-active" ]
                                       [ str "Ikke svart (@Model.OtherActivePlayers.Count())" ] ]
                                 div [ Id "ra-otherplayers-active"
                                       Class "collapse" ]
                                   [ str "@Html.Partial(_RegisterSquadListPlayers, Model.OtherActivePlayers)" ]
                                 br [ ]
                                 div [ Class "collapselink-parent" ]
                                   [ a [ Class "collapse-link collapsed"
                                         Role "button"
                                         DataToggle "collapse"
                                         Href "#ra-otherplayers-inactive" ]
                                       [ str "Øvrige ikke svart (@Model.OtherInactivePlayers.Count())" ] ]
                                 div [ Id "ra-otherplayers-inactive"
                                       Class "collapse" ]
                                   [ str "@Html.Partial(_RegisterSquadListPlayers, Model.OtherInactivePlayers)" ] ]
                             div [ Class "col-sm-6 col-xs-12 " ]
                               [ h2 [ ] [ str "Tropp (squadCount)" ]   
                                 hr [ ]
                                 div [ ]
                                     [ 
                                        ul [ Id "squad"; Class "list-unstyled squad-list" ] 
                                        //    "@foreach (var player in Model.Squad)"
                                            [
                                                li [ Id "@player.Id" ] [ 
                                                    i [ Class "flaticon-soccer18" ] []
                                                    str "&nbsp;@player.Name" ] ] 
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
                                        // @if (Model.Game.IsPublished)
                                         div [ Class "disabled btn btn-success btn-lg" ]
                                           [ i [ Class "fa fa-check-circle" ] [ ] 
                                             str "Publisert" ]
                                        //  "else"
                                         button [ Id "publishButton"
                                                  HTMLAttr.Custom ("data-
                                                  event-id", "@Model.Game.Id")
                                                  Class "btn btn-primary btn-lg" ]
                                           [ span [ ]
                                               [ str "Publiser
                                               tropp" ]
                                             i [ Class "fa fa-spinner fa-spin" ]
                                               [ ] ]
                                         span [ Class "label-feedback label label-danger" ]
                                           [ i [ Class "fa fa-exclamation-triangle" ]
                                               [ ] ] ] ] ] ]                    
                                           ]
        ]

ReactDom.render(element, document.getElementById(ClientViews.selectSquad))

