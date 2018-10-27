module MyTeam.Client.SelectSquad

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Import
open Fable.Import.Browser
open Fable.Import.React
open Fable.PowerPack
open MyTeam
open MyTeam.Client.Components
open MyTeam.Components
open MyTeam.Domain
open MyTeam.Shared.Components
open MyTeam.Shared.Components.Layout
open Shared.Features.Games.SelectSquad
open Thoth.Json


open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Import
open Fable.Import.React
open Fable.PowerPack
open Fable.PowerPack.Fetch
open MyTeam.Components
open MyTeam.Shared.Components
open MyTeam.Shared.Components.Input



type SubmitButtonState =
    { IsSubmitted: bool  
      IsPosting : bool
      Error : bool }

type SubmitButtonProps<'a> =
    { IsSubmitted : bool
      Text: string
      SubmittedText: string
      Url : string 
      Payload: 'a }

type SubmitButton<'a>(props) =
    inherit Component<SubmitButtonProps<'a>, SubmitButtonState>(props)
    
    do 
        base.setInitState({ IsSubmitted = props.IsSubmitted
                            Error = false
                            IsPosting = false })
        
    
    override this.render() =
        let handleClick _ =
            let props = this.props
            this.setState(fun state props -> 
                { state with Error = false
                             IsPosting = true })
            promise { 
                let! res = postRecord props.Url props.Payload []
                if not res.Ok then 
                    failwithf "Received %O from server: %O" res.Status res.StatusText
                this.setState(fun state props -> { state with IsPosting = false; IsSubmitted = true })
            }
            |> Promise.catch(fun e -> 
                   Browser.console.error <| sprintf "%O" e
                   this.setState(fun state props -> 
                       { state with Error = true
                                    IsPosting = false }))
            |> Promise.start                            

        let props = this.props
        let state = this.state

        let defaultButton attr content = 
            btn Primary Lg attr  content
                                    
        div [ Class "input-submit-button" ] [ 
                  (match state with
                      | { IsSubmitted = true } ->
                          btn 
                            Success 
                            Lg 
                            [ Class "disabled" ] 
                            [ Icons.checkCircle
                              whitespace
                              str props.SubmittedText ]
                      
                      | { IsPosting = true } -> 
                            defaultButton [ Class "disabled" ] [ Icons.spinner ]

                      | { Error = true } -> 
                            fragment [] [ 
                                defaultButton [ OnClick handleClick ] [ str props.Text ]
                                Labels.error
                            ]

                      | _ -> 
                            defaultButton [ OnClick handleClick ] [ str props.Text ]
                      )                                                 
                   
                
            ] 

                                        

let submitButton model = ofType<SubmitButton<'a>, _, _> model []


type SelectSquad(props) =
    inherit Component<Model, Squad>(props)
    do base.setInitState(props.Game.Squad)
    override this.render() =
        let model = { props with Game = { props.Game with Squad = this.state } }
        
        let getRecentAttendance memberId =
            model.RecentAttendance
            |> List.tryFind(fun a -> a.MemberId = memberId)
            |> function 
            | Some a -> sprintf "%i%%" a.AttendancePercentage
            | None -> "0%"
        
        let game = model.Game
        let imageOptions = model.ImageOptions
        
        let players : Player list =
            model.Members
            |> List.map(fun m -> 
                   let s = model.Signups |> List.tryFind(fun s -> s.MemberId = m.Details.Id)
                   (m, s))
        
        let handleSelectPlayer playerId isSelected =
            this.setState(fun state props -> 
                { state with MemberIds =
                                 if isSelected then state.MemberIds @ [ playerId ] |> List.distinct
                                 else state.MemberIds |> List.except [ playerId ] })
        
        
        let listPlayers header (players: Player list) =          
                 collapsible Open [
                     str <| sprintf "%s (%i)" header players.Length
                 ] [     
                    ul [ Class "list-users" ]
                        (players
                        |> List.map(fun (m, s) ->
                            let m = m.Details
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
                                    Checkbox.render 
                                        { Value = game.Squad.MemberIds |> List.contains m.Id
                                          Url = sprintf "/api/games/%O/squad/select/%O" game.Id m.Id
                                          OnChange = handleSelectPlayer m.Id }                    
                                    ]                                  
                                ]
                        ))
                    br []
                   ]               

        let squad =
            players |> List.filter(fun (m, _) -> game.Squad.MemberIds |> List.contains m.Details.Id)
        


        mtMain [] 
            [ block [] 
                  [ editLink <| sprintf "/intern/arrangement/endre/%O" game.Id
                    a [ Href "game/gameplan/gameId"
                        Class "registerSquad-gameplan-link pull-right"
                        Title "Bytteplan" ] [ Icons.gamePlan ]
                    
                    div [ Class "rs-header flex" ] 
                        [ div [ Class "flex-1 event-icon align-center flex-center" ] 
                              [ Icons.eventIcon EventType.Kamp Icons.ExtraLarge ]
                          div [ Class "flex-2 faded" ] [ p [] 
                                                             [ span [] [ Icons.calendar ""
                                                                         whitespace
                                                                         game.Date
                                                                         |> Date.format
                                                                         |> str ]
                                                               whitespace
                                                               span [ Class "no-wrap" ] [ whitespace
                                                                                          Icons.time ""
                                                                                          whitespace
                                                                                          game.Date
                                                                                          |> Date.formatTime
                                                                                          |> str ] ]
                                                         p [] [ Icons.mapMarker ""
                                                                whitespace
                                                                str game.Location ] ] ]
                    
                    div [ Class "row" ] 
                        [ div [ Class "col-sm-6 col-xs-12" ] 
                              [ listPlayers "Påmeldte spillere" 
                                    (players 
                                     |> List.filter(fun (_, s) -> s.IsSome && s.Value.IsAttending))
                                
                                listPlayers "Kan ikke" 
                                    (players 
                                     |> List.filter
                                            (fun (_, s) -> s.IsSome && (not s.Value.IsAttending)))
                                
                                listPlayers "Ikke svart" 
                                    (players 
                                     |> List.filter
                                            (fun (m, s) -> 
                                            not s.IsSome && (m.Teams |> List.contains game.TeamId)))
                                
                                listPlayers "Øvrige ikke svart" 
                                    (players 
                                     |> List.filter
                                            (fun (m, s) -> 
                                            not s.IsSome 
                                            && (not(m.Teams |> List.contains game.TeamId)))) ]
                          
                          div [ Class "col-sm-6 col-xs-12 " ] 
                              [ h2 [] [ str <| sprintf "Tropp (%i)" squad.Length ]
                                hr []
                                
                                div [] 
                                    [ ul [ Class "list-unstyled squad-list" ] 
                                          (squad
                                           |> List.map(fun (m, _) -> 
                                                  li [] [ Icons.player ""
                                                          str <| sprintf " %s" m.Details.Name ])) ]
                                hr []
                                
                                div [ Class "registerSquad-publish" ] 
                                    [ div [ Class "registerSquad-messageWrapper" ] 
                                          [ Textarea.render { Value = game.Description
                                                              Url = sprintf "/api/events/%O/description" game.Id } ]
                                      submitButton 
                                        { IsSubmitted = game.Squad.IsPublished
                                          Text = "Publiser tropp"
                                          SubmittedText = "Publisert"
                                          Url = sprintf "/api/games/%O/squad/publish" game.Id
                                          Payload = ignore }   
                                    
                                    ] ] ] ] ]

let element model = ofType<SelectSquad, _, _> model []
let node = document.getElementById(clientView)

if not <| isNull node then 
    node.getAttribute(Interop.modelAttributeName)
    |> Decode.Auto.fromString<Model>
    |> function 
    | Ok model -> ReactDom.render(element model, node)
    | Error e -> failwithf "Json deserialization failed: %O" e
