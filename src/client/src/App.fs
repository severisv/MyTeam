module FableApp

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Browser
open Fable.Helpers.React
open Fable.Helpers.React.Props
open MyTeam.Shared.Components

let init() =


    let element = div [ Class "mt-main" ]
                      [ div [ Class "mt-container" ]
                            [ h3 [ Class "col-xs-5" ]
                                [
                                  Icons.team ""
                                  str "Wam-Kam 1" ]
                              h3 [ Class "col-xs-6" ]
                                [ i [ Class "fa fa-calendar" ] [ ]
                                  str "2018" ]
                              a [ Class "btn btn-danger pull-right confirm-dialog"
                                  Title "Slett"
                                  HTMLAttr.Custom ("data-message", "Er du sikker på at du vil slette?")
                                  Href "/sesong/slett?seasonId=30c94276-275c-44cf-8f23-f7c06819c35e" ]
                                [ i [ Class "fa fa-trash" ]
                                    [ ] ]
                              div [ Class "col-xs-12" ]
                                  [ 
                                      hr [ ]  
                                      form  [ Class "update-table-form"
                                              Method "post"
                                              Action "/sesong/oppdater" ]                                             
                                            [
                                                div [ Class "form-group clearfix col-xs-8 col-sm-4 no-padding" ]
                                                    [ input [  Class "form-control"
                                                               Id "Name"
                                                               Name "Name"
                                                               Value "5. div. Menn avd. 01"
                                                               Type "text" ] 
                                                     ]
                                                br [ ]
                                                div [ Class "clearfix" ] [  ]
                                                div [ Class "form-group clearfix" ] [                                                                     
                                                        label [ Class "col-xs-4 no-padding control-label";HTMLAttr.Custom ("for", "AutoUpdate") ]
                                                              [ str "Oppdater tabell autom."
                                                                input [  Class "form-control"
                                                                         Name "AutoUpdate"
                                                                         Value "true"
                                                                         Type "checkbox" ] ]
                                                        div [ Class "col-xs-8 no-padding flex" ]
                                                            [ div [ Class "flex-2" ]
                                                                  [ span [ Id "AutoUpdateTableWrapper" ]
                                                                         [ 
                                                                             label [ Class "col-xs-6" ] [ str "URL til tabell på fotball.no" ]
                                                                             span [ Class "col-xs-6 " ]
                                                                                  [ input [ Class "form-control "
                                                                                            HTMLAttr.Custom ("data-val", "true")
                                                                                            HTMLAttr.Custom ("data-val-url", "Lenken må være en gyldig url")
                                                                                            Id "SourceUrl"
                                                                                            Name "SourceUrl"
                                                                                            Value "https://www.fotball.no/fotballdata/turnering/tabell/?fiksId=158436"
                                                                                            Type "text" ] ] ] ] ] ]
                                                               
                                                br [ ]
                                                div [ Class "clearfix" ]
                                                    [ br [ ] ]
                                                div [ Class "form-group" ]
                                                    [ button [ Type "submit"; Class "btn btn-primary" ] [ str "Lagre" ] ]
                                 
                                            ]
                                  ]
                               
                               ]] 
                               

    ReactDom.render(element, document.getElementById("main"))            
init()