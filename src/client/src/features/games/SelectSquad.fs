module Client.Games.SelectSquad

open System
open Shared.Domain.Members
open Shared.Image
open Fable.React
open Fable.React.Props
open Fable.React
open Shared
open Client.Components
open Shared.Components
open Shared.Domain
open Shared.Components.Base
open Shared.Components.Layout
open Shared.Util
open Shared.Domain.Members
open Thoth.Json
open Links

type Signup = {
        MemberId: Guid
        IsAttending: bool
        Message: string        
    }
    
type TeamAttendance = {
    MemberId: MemberId
    AttendancePercentage: int
}

type Player = MemberWithTeamsAndRoles * Signup option

type Squad = {
    MemberIds: Guid list
    IsPublished: bool
}

type GameDetailed = {
    Id: Guid
    Date: DateTime
    Location: string
    Description: string
    Squad: Squad
    TeamId: Guid
}


type Model = {
    Game: GameDetailed
    ImageOptions: CloudinaryOptions
    Signups: Signup list
    Members: MemberWithTeamsAndRoles list
    RecentAttendance: TeamAttendance list
}

let clientView = "select-squad"
let modelAttribute = "model"


type Category =
    | ``Påmeldte spillere``
    | ``Kan ikke``
    | ``Ikke svart``
    | ``Øvrige ikke svart``

let getCategory teamId =
    function
    | (_, Some signup) -> if signup.IsAttending then
                            ``Påmeldte spillere``
                          else
                              ``Kan ikke``
    | (m, _) when m.Teams |> List.contains teamId && m.Details.Status = PlayerStatus.Aktiv
         -> ``Ikke svart``
    | _ -> ``Øvrige ikke svart``
       
type SelectSquad(props) =
    inherit Component<Model, Squad>(props)
    do base.setInitState (props.Game.Squad)
    override this.render() =
        let model = { props with Game = { props.Game with Squad = this.state } }

        let getRecentAttendance memberId =
            model.RecentAttendance
            |> List.tryFind (fun a -> a.MemberId = memberId)
            |> function
            | Some a -> sprintf "%i%%" a.AttendancePercentage
            | None -> "0%"

        let game = model.Game
        let imageOptions = model.ImageOptions

        let players : Player list =
            model.Members
            |> List.map (fun m ->
                   let s = model.Signups |> List.tryFind (fun s -> s.MemberId = m.Details.Id)
                   (m, s))

        let handleSelectPlayer playerId isSelected =
            this.setState (fun state props ->
                { state with MemberIds =
                                 if isSelected then state.MemberIds @ [ playerId ] |> List.distinct
                                 else state.MemberIds |> List.except [ playerId ] })


        let listPlayers (category: Category) collapseState =
                 let players = players
                               |> List.filter(getCategory game.TeamId >> (=) category)
                 
                 collapsible collapseState [
                     str <| sprintf "%s (%i)" (string category) players.Length
                 ] [
                    ul [ Class "list-users" ]
                        (players                        
                        |> List.map (fun (m, s) ->
                            let m = m.Details
                            li [ Class "registerSquad-player" ]
                                [ span []
                                    [ img [ Class "hidden-xxs"
                                            Src <| Image.getMember imageOptions
                                                       (fun opts -> { opts with Height = Some 50; Width = Some 50 })
                                                       m.Image m.FacebookId
                                          ]
                                      str m.Name
                                    ]
                                  span [Style [Display DisplayOptions.Flex; JustifyContent "flex-end"; AlignItems AlignItemsOptions.Center] ] [
                                        s => fun s ->
                                            Strings.hasValue s.Message &?
                                                tooltip s.Message [ Class "registerSquad-messageIcon" ] [
                                                    Icons.comment
                                                ]
                                        span [ Title "Oppmøte siste 8 uker" ]
                                             [ str <| getRecentAttendance m.Id ]
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
            players |> List.filter (fun (m, _) -> game.Squad.MemberIds |> List.contains m.Details.Id)

        mtMain []
            [ block []
                  [ editAnchor [Href <| sprintf "/intern/arrangement/endre/%O" game.Id ]
                    a [ Href <| sprintf "/kamper/%O/bytteplan" game.Id
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
                              [ listPlayers ``Påmeldte spillere`` Open
                                listPlayers ``Kan ikke`` Open
                                listPlayers ``Ikke svart`` Open
                                listPlayers ``Øvrige ikke svart`` Collapsed ]

                          div [ Class "col-sm-6 col-xs-12 " ]
                              [ h2 [] [ str <| sprintf "Tropp (%i)" squad.Length ]
                                hr []

                                div []
                                    [ ul [ Class "list-unstyled squad-list" ]
                                          (squad
                                           |> List.map (fun (m, _) ->
                                                  li [] [ Icons.player ""
                                                          str <| sprintf " %s" m.Details.Name ])) ]
                                hr []

                                div [ Class "registerSquad-publish" ]
                                    [ div [ Class "registerSquad-messageWrapper" ]
                                          [ Textarea.render { Value = game.Description
                                                              Placeholder = Some "Beskjed til spillerne"
                                                              Url = sprintf "/api/events/%O/description" game.Id } ]

                                      Send.sendElement
                                        (fun o -> { o with
                                                      IsSent = Some game.Squad.IsPublished
                                                      SendElement = btn, [Lg; Primary], [str "Publiser tropp"]
                                                      SentElement = btn, [Lg; Success], [str "Publisert"]            
                                                      Endpoint = Send.Post (sprintf "/api/games/%O/squad/publish" game.Id, None) })
                                    ] ] ] ] ]


let element = ofType<SelectSquad, _, _>
ReactHelpers.render Decode.Auto.fromString<Model> clientView element
