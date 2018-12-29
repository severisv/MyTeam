namespace MyTeam.Table

open System.Linq
open MyTeam.Domain
open System
open MyTeam
open MyTeam.Strings

module Queries =

    let getYears : GetYears =
        fun db teamId ->
            query {
                for season in db.Seasons do
                where (season.TeamId = teamId) 
                select (season.StartDate.Year)
                distinct
            }
            |> Seq.toList
            |> List.sortDescending

    let getTable: GetTable =
        fun db teamId year ->

            let fromJson obj = 
                Json.fableDeserialize<TableRow list> obj
                |> function 
                | Ok value -> value 
                | Error e -> failwith e

            query {
                for season in db.Seasons do
                where (season.TeamId = teamId && season.StartDate.Year = year)
                select (season.TableJson, season.TableUpdated, season.Name, season.AutoUpdateTable, season.TableSourceUrl) 
            } 
            |> Seq.tryHead
            |> Option.map(fun (tableJson, updatedDate, name, autoUpdateTable, tableSourceUrl) ->
                    {
                        Rows =  if hasValue tableJson then
                                    fromJson tableJson
                                else 
                                    []
                        UpdatedDate = updatedDate
                        Title = !!name
                        AutoUpdate = autoUpdateTable
                        SourceUrl = !!tableSourceUrl 
                    }
                )
    