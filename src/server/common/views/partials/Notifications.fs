namespace MyTeam.Views

open Giraffe.ViewEngine
open MyTeam
open Shared.Domain.Members
open Shared.Components
open MyTeam.Views.BaseComponents

module NotificationViews =

    let internal content ctx club user =
        let model = Notifications.get ctx club user

        (if model.UnansweredEvents > 0 then
             li [ _class "dropdown" ] [
                 button [ _class "dropdown-toggle btn btn-warning"
                          attr "data-toggle" "dropdown" ] [
                     icon <| fa "bell-o" <| ""
                 ]
                 ul [ _class "dropdown-menu" ] [
                     li [] [
                         a [ _href
                             <| sprintf "/intern#event-%O" (model.UnansweredEventIds |> Seq.head) ] [
                             !!(Icons.signup "")
                             whitespace
                             whitespace
                             span [ _class "hidden-xxs" ] [
                                 encodedText "Du har "
                             ]
                             encodedText
                             <| sprintf
                                 "%i %s"
                                 model.UnansweredEvents
                                 (model.UnansweredEvents > 1
                                  =? (" ubesvarte arrangementer", " ubesvart arrangement"))
                         ]
                     ]
                 ]
             ]
         else
             emptyText)

    let notifications club (user: User) (ctx: HttpContext) =
        ul [ _id "notification-button"
             _class "notification-button nav navbar-nav navbar-right navbar-topRight--item" ] [
            content ctx club user
        ]

    let notificationPartial club (user: User) (ctx: HttpContext) = content ctx club user |> OkResult
