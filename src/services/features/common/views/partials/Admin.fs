module MyTeam.Views.Admin

open Giraffe.GiraffeViewEngine
open MyTeam
open MyTeam.Domain.Members
open MyTeam.Views


let coachMenuItems = [
    li [][a [_href "/intern/arrangement/ny"] [Icons.training ""; encodedText "Opprett arrangement"]]
    li [][a [_href "/admin"] [Icons.user ""; encodedText "Administrer spillere"]]
    li [][a [_href "/admin/spillerinvitasjon"] [icon <| fa "plus" <| ""; encodedText "Legg til spiller"]]
    li [][a [_href "/nyheter/ny"] [Icons.news ""; encodedText "Skriv artikkel"]]
]

let coachMenu =
    div [_class "mt-sidebar"] [
        block [] [
            ul [_class "nav nav-list adminMenu"] 
                ([
                  li [_class "nav-header"] [encodedText "Adminmeny"] 
                 ] @ coachMenuItems)

        ]
    ]
