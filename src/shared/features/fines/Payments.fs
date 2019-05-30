module Shared.Features.Fines.Payments
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
        sprintf "/intern/boter/innbetalinger/%s%s" year memberId


type Payment = {
    Id: Guid
    Member: Member
    Comment: string
    Amount: int
    Date: DateTime
 }


type PaymentsModel = {
    ImageOptions: CloudinaryOptions
    Payments: Payment list
    Year: SelectedYear
    SelectedMember: SelectedMember
    User: User
    Path: string
    Years: int list
    Members: Member list
    PaymentInformation: string
}

[<CLIMutable>]
type AddPayment = {
    Id: Guid option
    MemberId: Guid
    Date: DateTime
    Amount: int
    Comment: string
}

let paymentsView = "list-payments"
