module Shared.Features.Admin.AddPlayers

open Shared.Image


[<CLIMutable>]
type AddMemberForm = {
    FacebookId: string 
    ``E-postadresse``: string 
    Fornavn: string
    Mellomnavn: string
    Etternavn: string
}

type Model = {
    ImageOptions: CloudinaryOptions
    MemberRequests: AddMemberForm list
}

let clientView = "addplayers"
let modelAttribute = "model"