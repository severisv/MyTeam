module Shared.Features.Fines
open Fable.Import.React
open Shared.Image
open Shared
open System
open Shared.Domain.Members
open Shared.Components
open Shared.Components.Tabs

type SelectedYear =
    | AllYears
    | Year of int

type SelectedMember =
    | AllMembers
    | Member of Guid

let createUrl year memberId =
        let year = match year with
                    | AllYears -> "total"
                    | Year year -> string year
        let memberId = match memberId with
                        | Member id -> sprintf "/%O" id
                        | AllMembers -> ""            
        sprintf "/intern/boter/vis/%s%s" year memberId


type Fine = {
    Id: Guid
    Member: Member
    Description: string
    Amount: int
    Issued: DateTime
 }

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

let fineNav (user : User) (currentPath : string) =
    tabs []
       ([
        { Text = " Oversikt"
          ShortText = " Oversikt"
          Url = "/intern/boter/oversikt"
          Icon = Some (Icons.barChart "") }
        { Text = " Alle bøter"
          ShortText = " Bøter"
          Url = "/intern/boter/vis"
          Icon = Some (Icons.fine "") }
        { Text = " Bøtesatser"
          ShortText = " Bøtesatser"
          Url = "/intern/boter/satser"
          Icon = Some (Icons.dollar "") }
       ] @ (if user.IsInRole [ Role.Botsjef ] then
                [ { Text = " Innbetalinger"
                    ShortText = " Innbetalinger"
                    Url = "/intern/innbetalinger"
                    Icon = Some (Icons.list "") } ]
            else []
                )) (currentPath.Contains)