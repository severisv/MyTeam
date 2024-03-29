namespace MyTeam.Attendance.Pages

open Giraffe.ViewEngine
open Fable.React.Props
open Shared.Components
open Links
open MyTeam
open Shared
open Shared.Domain
open Shared.Domain.Events
open MyTeam.Views
open MyTeam.Attendance.Queries
open MyTeam.Attendance
open MyTeam.Views.BaseComponents
open Client.Components

module Register =

    let view (club: Club) user trainingId (ctx: HttpContext) =
        let db = ctx.Database

        let previousTrainings = getPreviousTrainings db club.Id

        let getTraining = getTraining db


        let model =
            let getModel (training: Event) =
                let players = getPlayers db club.Id training.Id
                (training, players)

            match trainingId with
            | Some trainingId ->
                match getTraining trainingId with
                | Some training -> getModel training |> Some
                | None -> None
            | None ->
                previousTrainings
                |> List.tryHead
                |> Option.map getModel

        let registerAttendanceUrl (training: Event option) =
            match training with
            | Some training -> sprintf "/intern/oppmote/registrer/%s" (string training.Id)
            | None -> "/intern/oppmote/registrer"

        let isSelected url =
            registerAttendanceUrl (
                model
                |> Option.map (fun (training, _) -> training)
            ) = url

        let getImage = Images.getMember ctx

        let editEventLink eventId =
            !!(editAnchor [
                Href <| sprintf "/treninger/%O/endre" eventId
               ])

        let registerAttendancePlayerList header (players: PlayerAttendance list) (selectedEvent: Event) isCollapsed =
            collapsible
                isCollapsed
                [ encodedText
                  <| sprintf "%s (%i)" header players.Length ]
                [ div [] [
                      ul
                          [ _class "list-users" ]
                          (players
                           |> List.map (fun (p, didAttend, didWin) ->
                               li [ _class "register-attendance-item" ] [
                                   span [] [
                                       img [
                                           _src
                                           <| getImage
                                               (fun o ->
                                                   { o with
                                                       Height = Some 50
                                                       Width = Some 50 })
                                               p.Image
                                               p.FacebookId
                                       ]
                                       encodedText p.Name
                                   ]
                                   //                   ajaxCheckbox (sprintf "/api/attendance/%O/registrer/%O/victory" selectedEvent.Id p.Id) didWin
                                   //                       |> withClass "register-trainingVictory"
                                   Client.comp
                                       AutoSync.Checkbox.Element
                                       { Value = didAttend
                                         Url = $"/api/attendance/{selectedEvent.Id}/registrer/{p.Id}"
                                         OnChange = None }
                               ]))
                  ] ]

        match model with
        | Some model ->

            [ mtMain [ _class "mt-main--narrow register-attendance" ] [
                  block
                      []
                      (model
                       |> fun (training, players) ->
                           [ editEventLink training.Id
                             div [ _class "attendance-event" ] [
                                 !!(Icons.eventIcon EventType.Trening Icons.ExtraLarge)
                                 div [ _class "faded" ] [
                                     p [] [
                                         icon (fa "calendar") ""
                                         whitespace
                                         encodedText
                                         <| (training.Date.ToString("ddd d MMMM"))
                                     ]
                                     p [] [
                                         icon (fa "map-marker") ""
                                         encodedText training.Location
                                     ]
                                 ]
                             ]
                             registerAttendancePlayerList "Påmeldte spillere" players.Attending training Open
                             br []
                             registerAttendancePlayerList "Øvrige aktive" players.OthersActive training Collapsed
                             br []
                             registerAttendancePlayerList "Øvrige inaktive" players.OthersInactive training Collapsed ])
              ]
              sidebar [] [
                  (previousTrainings.Length > 0
                   =? (block [] [
                           !!(Nav.navList (
                               { Header = "Siste treninger"
                                 Items =
                                   previousTrainings
                                   |> List.map (fun training ->
                                       { Text =
                                           [ Icons.calendar ""
                                             Base.whitespace
                                             Fable.React.Helpers.str
                                             <| (training.Date.ToString "ddd d MMMM") ]
                                         Url = registerAttendanceUrl (Some training) })
                                 Footer = None
                                 IsSelected = isSelected }
                           ))
                       ],
                       emptyText))
              ] ]
            |> layout club (Some user) (fun o -> { o with Title = "Registrer oppmøte" }) ctx
            |> OkResult
        | None -> NotFound
