module Shared.Features.Fines.Add

open System
open Shared.Domain.Members
open Shared.Features.Fines.Common

[<CLIMutable>]
type AddFine = {
    MemberId: Guid option
    Date: DateTime
    RateId: Guid option
    ExtraRate: int option
    Comment: string
}

