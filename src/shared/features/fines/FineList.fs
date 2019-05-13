module Shared.Features.Fines.List
open Shared.Features.Fines.Common
open Shared.Image
open Shared.Domain.Members

let createUrl year memberId =
        let year = match year with
                    | AllYears -> "total"
                    | Year year -> string year
        let memberId = match memberId with
                        | Member id -> sprintf "/%O" id
                        | AllMembers -> ""            
        sprintf "/intern/boter/vis/%s%s" year memberId



type ListModel = {
    ImageOptions: CloudinaryOptions
    Fines: Fine list
    Year: SelectedYear
    SelectedMember: SelectedMember
    User: User
    Path: string
    Years: int list
    Members: Member list
}

let listView = "list-fines"
