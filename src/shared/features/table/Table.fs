module Shared.Features.Table.Table

open System
open MyTeam
open MyTeam.Domain.Members
open MyTeam.Image


type EditModel = {
    Title: string
    Team: string
    Year: int
    AutoUpdateTable: bool
    SourceUrl: string
}

type CreateModel = {
    Team: string
}
let editView = "edit-table"
let createView = "create-table"
let modelAttribute = "model"