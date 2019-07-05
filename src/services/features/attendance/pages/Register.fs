namespace MyTeam.Attendance.Pages

open Giraffe.GiraffeViewEngine
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
open MyTeam.Ajax
open MyTeam.Views.BaseComponents

module Register =

    let view (club : Club) user trainingId (ctx : HttpContext) =
        let db = ctx.Database

        let previousTrainings =
            getPreviousTrainings db club.Id

        let getTraining =
            getTraining db

        let model =
            let getModel (training : Event) =
                let players = getPlayers db club.Id training.Id
                (training, players)

            match trainingId with
            | Some trainingId -> getTraining trainingId |> getModel |> Some
            | None -> previousTrainings
                      |> List.tryHead
                      |> Option.map getModel

        let registerAttendanceUrl (training : Event option) =
            match training with
            | Some training -> sprintf "/intern/oppmote/registrer/%s" (string training.Id)
            | None -> "/intern/oppmote/registrer"

        let isSelected url =
            registerAttendanceUrl (model |> Option.map (fun (training, _) -> training)) = url

        let getImage = Images.getMember ctx

        let editEventLink eventId =
            !!(editAnchor [Href <| sprintf "/intern/arrangement/endre/%s" (string eventId)])

        let registerAttendancePlayerList header (players : PlayerAttendance list) (selectedEvent : Event) isCollapsed =
            collapsible
                isCollapsed
                [ encodedText <| sprintf "%s (%i)" header players.Length ]
                [
                    div [ _class "row" ] [
                        ul [ _class "list-users col-xs-11 col-md-10" ]
                            (players |> List.map (fun (p, didAttend, didWin) ->
                                li [ _class "register-attendance-item" ] [
                                    span [] [
                                        img [ _src <| getImage (fun o -> { o with Height = Some 50
                                                                                  Width = Some 50 }) p.Image p.FacebookId ]
                                        encodedText p.Name
                                        ]
                                    ajaxCheckbox (sprintf "/api/attendance/%O/registrer/%O/victory" selectedEvent.Id p.Id) didWin
                                        |> withClass "register-trainingVictory"
                                    ajaxCheckbox (sprintf "/api/attendance/%O/registrer/%O" selectedEvent.Id p.Id) didAttend
                                    ajaxSuccessIndicator
                                ]
                            ))
                        ]
                ]
        [
            mtMain [ _class "mt-main--narrow register-attendance" ] [
                block []
                        (model
                        |> Option.fold (fun _ (training, players) ->
                            [
                                editEventLink training.Id
                                div [ _class "attendance-event" ] [
                                    !!(Icons.eventIcon EventType.Trening Icons.ExtraLarge)
                                    div [ _class "faded" ] [
                                        p [] [
                                            icon (fa "calendar") ""
                                            whitespace
                                            encodedText <| (training.Date.ToString("ddd d MMMM"))
                                        ]
                                        p [] [ icon (fa "map-marker") ""
                                               encodedText training.Location ] ]
                                ]
                                registerAttendancePlayerList "Påmeldte spillere" players.Attending training Open
                                br []
                                registerAttendancePlayerList "Øvrige aktive" players.OthersActive training Collapsed
                                br []
                                registerAttendancePlayerList "Øvrige inaktive" players.OthersInactive training Collapsed
                            ])
                            [ emptyText ]
                            )
            ]
            sidebar [] [
                (previousTrainings.Length > 0 =?
                    (block [] [
                        !!(Nav.navList ({
                                    Header = "Siste treninger"
                                    Items = previousTrainings
                                            |> List.map (fun training ->
                                                            { Text = [ Icons.calendar ""
                                                                       Base.whitespace
                                                                       Fable.React.Helpers.str <| (training.Date.ToString "ddd d MMMM") ]
                                                              Url = registerAttendanceUrl (Some training) })
                                    Footer = None
                                    IsSelected = isSelected }))
                    ]
                   , emptyText))
            ]
        ]
        |> layout club (Some user) (fun o -> { o with Title = "Registrer oppmøte" }) ctx
        |> OkResult
