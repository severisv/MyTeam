module Shared.Features.Fines.Add

open System
open Shared.Domain.Members
open Shared.Features.Fines.Common

[<CLIMutable>]
type AddFine = {
    MemberId: Guid
    Date: DateTime
    RateId: Guid
    ExtraRate: int option
    Comment: string
}

