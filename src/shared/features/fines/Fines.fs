module Shared.Features.Fines.Common
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

type RemedyRate = {
    Id: Guid
    Amount: int
    Name: string
}

type Fine = {
    Id: Guid
    Member: Member
    Description: string
    Amount: int
    Issued: DateTime
 }

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
       ] @ (if user.IsInRole [Role.Botsjef] then
                [ { Text = " Innbetalinger"
                    ShortText = " Innbetalinger"
                    Url = "/intern/innbetalinger"
                    Icon = Some (Icons.list "") } ]
            else []
                )) (currentPath.Contains)