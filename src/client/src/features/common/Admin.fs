module Client.Features.Common.Admin

open Fable.React
open Fable.React.Props
open Shared.Components
open Shared.Components.Layout

let coachMenuItems = [
    li [][a [Href "/intern/arrangement/ny"] [Icons.training ""; str "Opprett arrangement"]]
    li [][a [Href "/admin"] [Icons.user ""; str "Administrer spillere"]]
    li [][a [Href "/admin/spillerinvitasjon"] [Icons.add ""; str "Legg til spiller"]]
    li [][a [Href "/nyheter/ny"] [Icons.news ""; str "Skriv artikkel"]]
]

let coachMenu =
    sidebar [] [
        block [] [
            ul [Class "nav nav-list adminMenu"] 
                ([
                  li [Class "nav-header"] [str "Admin"] 
                 ] @ coachMenuItems)

        ]
    ]