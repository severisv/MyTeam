module Shared.Features.Fines.RemedyRates
open Shared.Domain.Members
open Shared.Features.Fines.Common


type RemedyRatesModel = {
    Rates: RemedyRate list
    Path: string
    User: User
}

let ratesView = "remedyrates"
