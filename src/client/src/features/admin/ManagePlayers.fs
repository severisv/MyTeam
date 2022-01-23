module Client.Features.Admin.ManagePlayers



open Shared.Util
open Feliz
open Shared
open System
open Thoth.Json
open Shared.Domain
open Shared.Domain.Members
open Shared.Components
open Client.Util
open Client.Features.Games.Common
open Shared.Components.Tables
open Shared.Components.Labels
open Shared.Image

let containerId = "manage-players"

let fragment = Fable.React.Helpers.fragment

type Props =
    { Members: MemberWithTeamsAndRoles list
      Teams: Team list
      ImageOptions: CloudinaryOptions }

[<ReactComponent>]
let ManagePlayers (props: Props) =

    let (members, setMembers) = React.useState (props.Members)

    members
    |> List.groupBy (fun m -> m.Details.Status)
    |> List.sortBy (fun (key, _) -> key)
    |> List.map (fun (key, members) ->
        Html.div [
            Html.h2 [
                prop.style [ style.textAlign.left ]
                prop.children [
                    Html.text (string key)
                    Html.span [
                        prop.style [
                            style.fontSize (length.px 22)
                        ]
                        prop.children [
                            Html.text $" ({members.Length})"
                        ]
                    ]

                    ]
            ]
            table
                [ Striped
                  (Attribute(
                      Fable.React.Props.Style [
                          Fable.React.Props.CSSProp.MarginBottom "40px"
                      ]
                  )) ]
                [ col [ CellType Image
                        NoSort
                        Visible(ScreenLg) ] []
                  col [ NoSort ] [ Html.text "Navn" ]
                  col [ NoSort ] [ Html.text "Lag" ]
                  col [ NoSort ] [ Html.text "Roller" ]
                  col [ NoSort ] [ Html.text "Status" ]
                  col [ NoSort ] [] ]
                (members
                 |> List.map (fun m ->
                     tableRow [] [
                         Html.img [
                             prop.src
                             <| Image.getMember
                                 props.ImageOptions
                                 (fun o ->
                                     { o with
                                         Height = Some 40
                                         Width = Some 40 })
                                 m.Details.Image
                                 m.Details.FacebookId
                         ]

                         Html.text m.Details.FullName

                         Html.div [
                             prop.style [
                                 style.display.flex
                                 style.gap (length.px 2, length.px 2)
                             ]
                             prop.children (
                                 props.Teams
                                 |> List.map (fun t ->
                                     let team =
                                         m.Teams |> List.tryFind (fun team -> team = t.Id)

                                     btn [ (team
                                            |> Option.map (fun _ -> Primary)
                                            |> Option.defaultValue Default)
                                           Sm
                                           Fable.React.Props.OnClick(fun _ -> ()) ] [
                                         Html.text (t.ShortName)
                                     ]

                                 )
                             )
                         ]


                         Html.div [
                             prop.style [
                                 style.display.flex
                                 style.gap (length.px 2, length.px 2)
                             ]
                             prop.children (
                                 (m.Roles
                                  |> List.map (fun role ->
                                      label
                                          LightBlue
                                          [ Html.text (string role)
                                            Icons.remove ]))
                                 @ [ Html.button [
                                         prop.style [
                                             style.marginLeft (length.px 2)
                                             style.fontSize (length.px 10)
                                         ]
                                         prop.children [
                                             Icons.add "Tildel ny rolle"
                                         ]
                                     ] ]
                             )
                         ]
                         btn [ Default
                               Sm
                               Fable.React.Props.Style [
                                   Fable.React.Props.CSSProp.MarginRight "2px"
                               ]
                               Fable.React.Props.OnClick(fun _ -> ()) ] [
                             Icons.chevronDown
                         ]
                         Html.a [ Icons.edit "Rediger profil" ]
                     ]))


            ]

    )
    |> Html.div




ReactHelpers.render2 Decode.Auto.fromString<Props> containerId ManagePlayers
