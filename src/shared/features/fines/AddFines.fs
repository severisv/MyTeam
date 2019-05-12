module Shared.Features.Fines.Add

open System
open Shared.Domain.Members
open Shared.Features.Fines.Common

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

