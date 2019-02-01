module Shared.Features.Admin.AddPlayers

open System
open MyTeam


[<CLIMutable>]
type AddMemberForm = {
    FacebookId: string 
    ``E-postadresse``: string 
    Fornavn: string
    Mellomnavn: string
    Etternavn: string
}


type Model = {
    Year: int option
}

let clientView = "addplayers"
let modelAttribute = "model"