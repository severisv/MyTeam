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
            query {
                for season in db.Seasons do
                where (season.TeamId = teamId && season.StartDate.Year = year)
                select (season.TableJson, season.TableUpdated, season.Name) 
            } 
            |> Seq.tryHead
            |> Option.map(fun (tableJson, updatedDate, name) ->
                    {
                        Rows =  if hasValue tableJson then
                                    Table.fromJson tableJson
                                else 
                                    []
                        UpdatedDate = updatedDate
                        Title = name
                    }
                )
    