namespace MyTeam.Table

open MyTeam
open MyTeam.Domain
open MyTeam.Strings
open System
open System.Linq

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
    
    let getTable : GetTable =
        fun db teamId year -> 
            let fromJson obj =
                Json.fableDeserialize<TableRow list> obj
                |> function 
                | Ok value -> value
                | Error e -> failwith e
            query { 
                for season in db.Seasons do
                    where (season.TeamId = teamId && season.StartDate.Year = year)
                    select 
                        (season.TableJson, season.TableUpdated, season.Name, season.AutoUpdateTable, 
                         season.TableSourceUrl, season.AutoUpdateFixtures, season.FixturesSourceUrl)
            }
            |> Seq.tryHead
            |> Option.map (fun (tableJson, updatedDate, name, autoUpdateTable, tableSourceUrl, autoUpdateFixtures, fixtureSourceUrl) -> 
                   { Rows =
                         if hasValue tableJson then fromJson tableJson
                         else []
                     UpdatedDate = updatedDate
                     Title = !!name
                     AutoUpdate = autoUpdateTable
                     SourceUrl = !!tableSourceUrl
                     FixtureSourceUrl = !!fixtureSourceUrl
                     AutoUpdateFixtures = autoUpdateFixtures })
