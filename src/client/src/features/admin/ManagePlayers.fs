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
open Shared.Components.Tables
open Shared.Components.Labels
open Shared.Image
open Fetch
open Client.Components.DropdownOptions

let containerId = "manage-players"

let fragment = Fable.React.Helpers.fragment

type Props =
    { Members: MemberWithTeamsAndRoles list
      Teams: Team list
      ImageOptions: CloudinaryOptions }



[<ReactComponent>]
let ManagePlayers (props: Props) =

    let (members, setMembers) = React.useState (props.Members)

    let toggleTeam playerId teamId _ =
        promise {
            setMembers (
                members
                |> List.mapWhen (fun m -> m.Details.Id = playerId) (fun m -> { m with Teams = m.Teams |> List.toggle teamId })
            )

            let! res = Http.sendRecord HttpMethod.PUT $"api/members/{playerId}/toggleteam" {| TeamId = teamId |} []

            if not res.Ok then
                setMembers members
                failwithf "Received %O from server: %O" res.Status res.StatusText

            ()
        }
        |> Promise.catch (fun e -> Browser.Dom.console.error (sprintf "%O" e))
        |> Promise.start


    let toggleRole playerId role _ =
        promise {
            setMembers (
                members
                |> List.mapWhen (fun m -> m.Details.Id = playerId) (fun m -> { m with Roles = m.Roles |> List.toggle role })
            )

            let! res = Http.sendRecord HttpMethod.PUT $"api/members/{playerId}/togglerole" {| Role = role |} []

            if not res.Ok then
                setMembers members

                failwithf "Received %O from server: %O" res.Status res.StatusText

            ()
        }
        |> Promise.catch (fun e -> Browser.Dom.console.error (sprintf "%O" e))
        |> Promise.start


    let setStatus playerId status _ =
        promise {
            setMembers (
                members
                |> List.mapWhen (fun m -> m.Details.Id = playerId) (fun m -> { m with Details = { m.Details with Status = status } })
            )

            let! res = Http.sendRecord HttpMethod.PUT $"api/members/{playerId}/status" {| Status = status |} []

            if not res.Ok then
                setMembers members
                failwithf "Received %O from server: %O" res.Status res.StatusText

            ()
        }
        |> Promise.catch (fun e -> Browser.Dom.console.error (sprintf "%O" e))
        |> Promise.start


    members
    |> List.groupBy (fun m -> m.Details.Status)
    |> List.sortBy (fun (key, _) -> key)
    |> List.map (fun (key, members) ->
        Html.div [
            prop.children [

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
                             Html.a [
                                 prop.href $"/spillere/vis/{m.Details.UrlName}"
                                 prop.children [
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
                                 ]
                             ]
                             Html.a [
                                 prop.href $"/spillere/vis/{m.Details.UrlName}"
                                 prop.className "black"
                                 prop.children [
                                     Html.text m.Details.FullName
                                 ]
                             ]
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
                                               Fable.React.Props.OnClick(toggleTeam m.Details.Id t.Id) ] [
                                             Html.text (t.ShortName)
                                         ]

                                     )
                                 )
                             ]


                             Html.div [
                                 prop.style [
                                     style.display.flex
                                     style.flexWrap.wrap
                                     style.gap (length.px 2, length.px 2)
                                 ]
                                 prop.children (
                                     (m.Roles
                                      |> List.map (fun role ->
                                          label
                                              LightBlue
                                              [ Html.text (string role)
                                                Html.text " "
                                                Html.button [
                                                    prop.onClick <| toggleRole m.Details.Id role
                                                    prop.children [ Icons.remove ]
                                                ] ]))
                                     @ [ dropDownOptions
                                             (allRoles
                                              |> List.except m.Roles
                                              |> List.map (fun role ->
                                                  { Label = role |> string |> Html.text
                                                    OnClick = toggleRole m.Details.Id role }))
                                             (fun toggleOptions ->
                                                 if allRoles |> List.except m.Roles |> List.isEmpty then
                                                     Html.text ""
                                                 else
                                                     Html.button [
                                                         prop.style [
                                                             style.marginLeft (length.px 2)
                                                             style.fontSize (length.px 10)
                                                             style.minHeight (length.px 26)
                                                             style.minWidth (length.px 26)
                                                         ]
                                                         prop.onClick toggleOptions
                                                         prop.children [
                                                             Icons.add "Tildel ny rolle"
                                                         ]
                                                     ]) ]
                                 )
                             ]
                             dropDownOptions
                                 (PlayerStatus.all
                                  |> List.except [ m.Details.Status ]
                                  |> List.map (fun status ->
                                      { Label = status |> string |> Html.text
                                        OnClick = setStatus m.Details.Id status }))
                                 (fun toggleOptions ->
                                     btn [ Default
                                           Sm
                                           Fable.React.Props.OnClick toggleOptions ] [
                                         Icons.chevronDown
                                     ])


                         ]))


                ]
        ])
    |> Html.div


ReactHelpers.render2 Decode.Auto.fromString<Props> containerId ManagePlayers
