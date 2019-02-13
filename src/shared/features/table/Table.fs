module Shared.Features.Table.Table


type EditModel = {
    Title: string
    Team: string
    Year: int
    AutoUpdateTable: bool
    SourceUrl: string
    AutoUpdateFixtures: bool
    FixtureSourceUrl: string
}

type CreateModel = {
    Team: string
}
let editView = "edit-table"
let createView = "create-table"
let modelAttribute = "model"