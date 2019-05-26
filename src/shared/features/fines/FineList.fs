module Shared.Features.Fines.List
open Shared.Features.Fines.Common
open Shared.Image
open Shared.Domain.Members
open System

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

[<CLIMutable>]
type AddFine = {
    Id: Guid option
    MemberId: Guid
    Date: DateTime
    RateId: Guid
    RateName: string
    Amount: int
    Comment: string
}

let listView = "list-fines"
