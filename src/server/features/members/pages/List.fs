module MyTeam.Members.Pages.List

open Giraffe.ViewEngine
open MyTeam
open MyTeam.Attendance
open MyTeam.Common.Features.Members
open Shared.Domain
open Shared.Domain.Members
open MyTeam.Members
open MyTeam.Views
open Shared
open TableModule

let view (club: Club) (user: User) status (ctx: HttpContext) =
    let status =
        match status with
        | Some s ->
            Enums.tryFromString<Status> s
            |> function
                | Ok s -> s
                | Error _ -> Status.Aktiv
        | None -> Status.Aktiv

    let members =
        Queries.listMembersDetailed ctx.Database club.Id

    let memberListUrl status = sprintf "/intern/lagliste/%s" status
    let getImage = Images.getMember ctx

    let isSelected =
        (equals <| memberListUrl (status |> toLowerString))

    [ mtMain [] [
          block [] [
              !!(Components.Tabs.tabs
                  []
                  ([ Status.Aktiv
                     Status.Veteran
                     Status.Inaktiv ]
                   |> List.map (fun status ->
                       { Text = status |> string
                         ShortText = status |> string
                         Url = memberListUrl (string status |> toLower)
                         Icon =
                           Some
                           <| Shared.Components.Icons.playerStatusIcon status }))
                  isSelected)
              br []

              table
                  []
                  [ col [ NoSort
                          CellType Image
                          Attr <| _class "hidden-xs" ] [
                      encodedText ""
                    ]
                    col [] [ encodedText "Navn" ]
                    col [] [ encodedText "Telefon" ]
                    col [ Attr <| _class "hidden-xs" ] [
                        encodedText "E-post"
                    ]
                    col [ Attr <| _class "visible-lg" ] [
                        encodedText "Født"
                    ] ]
                  (members
                   |> List.filter (fun m -> m.Details.Status = status)
                   |> List.map (fun m ->
                       tableRow [] [
                           a [ _href $"/spillere/vis/{m.Details.UrlName}" ] [
                               img [
                                   _src
                                   <| getImage
                                       (fun o ->
                                           { o with
                                               Height = Some 50
                                               Width = Some 50 })
                                       m.Details.Image
                                       m.Details.FacebookId
                               ]
                           ]

                           a [ _href $"/spillere/vis/{m.Details.UrlName}"
                               _class "black" ] [
                               encodedText m.Details.FullName
                           ]

                           a [ _href <| sprintf "tel:%s" m.Phone ] [
                               encodedText m.Phone
                           ]

                           a [ _href <| sprintf "mailto:%s" m.Email ] [
                               encodedText m.Email
                           ]
                           encodedText (m.BirthYear |> toString)
                       ]))
          ]
      ] ]
    |> layout club (Some user) (fun o -> { o with Title = "Lagliste" }) ctx
    |> OkResult
