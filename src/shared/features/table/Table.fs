module Shared.Features.Table.Table

open System
open MyTeam
open MyTeam.Domain.Members
open MyTeam.Image


type Model = {
    Title: string
    Team: string
    Year: int
    AutoUpdateTable: bool
    SourceUrl: string
}

let clientView = "table"
let modelAttribute = "model"