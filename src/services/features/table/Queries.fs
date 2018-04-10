namespace MyTeam.Table

open System.Linq
open MyTeam.Domain
open System

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
            
            query {
                for season in db.Seasons do
                where (season.TeamId = teamId && season.StartDate.Year = year)
                select (season.TableString, season.TableUpdated, season.Name) 
            } 
            |> Seq.tryHead
            |> Option.map(fun (tableString, updatedDate, name) ->
                    {
                        Rows = Table.fromString tableString
                        UpdatedDate = updatedDate
                        Title = name
                    }
                )
    