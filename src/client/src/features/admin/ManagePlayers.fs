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

let containerId = "manage-players"

type Props =
    { Members: MemberWithTeamsAndRoles list
      Teams: Team list }

[<ReactComponent>]
let ManagePlayers (props: Props) =

    let (members, setMembers) = React.useState (props.Members)

    Html.div (
        members
        |> List.map (fun m -> Html.div m.Details.FullName)
    )

ReactHelpers.render2 Decode.Auto.fromString<Props> containerId ManagePlayers
