module MyTeam.Table.Api

open MyTeam
open Shared
open Shared.Domain
open MyTeam.Models
open Shared.Components.Input
open System
open System.Linq

let internal update (club : Club) (teamName, year) (ctx : HttpContext) updateFn =
    let db = ctx.Database
    club.Teams.Where (fun t -> (t.ShortName |> Strings.toLower) = (teamName |> toLower))
    |> Seq.tryHead
    |> function 
    | None -> NotFound
    | Some team ->
        let startDate = DateTime(year, 01, 01)
        let endDate = DateTime(year, 12, 31)
        db.Seasons.Where(fun s -> s.TeamId = team.Id && s.StartDate >= startDate && s.StartDate <= endDate )
        |> Seq.tryHead 
        |> function 
        | None -> 
            let season =
                Models.Domain.Season
                    (StartDate = startDate, EndDate = endDate, Name = "", 
                     TeamId = team.Id, TableSourceUrl = "", TableUpdated = DateTime.Now)
            db.Seasons.Add(season) |> ignore
            season
        | Some s -> s
        |> fun season -> 
            updateFn season
            db.SaveChanges() |> ignore
            OkResult None


let create club teamNameYear ctx _ =
    update club teamNameYear ctx (fun season -> season.TableUpdated <- DateTime.Now)
    
let setTitle club teamNameYear ctx (model: {| Value: string |}) =
    update club teamNameYear ctx (fun season -> season.Name <- model.Value)
    
let setSourceUrl club teamNameYear ctx (model: {| Value: string |}) =
    update club teamNameYear ctx (fun season -> season.TableSourceUrl <- model.Value)
    
let setAutoUpdate club teamNameYear ctx (model : CheckboxPayload) =
    update club teamNameYear ctx (fun season -> season.AutoUpdateTable <- model.value)
    
let setFixtureSourceUrl club teamNameYear ctx (model: {| Value: string |}) =
    update club teamNameYear ctx (fun season -> season.FixturesSourceUrl <- model.Value)
    
let setAutoUpdateFixtures club teamNameYear ctx (model : CheckboxPayload) =
    update club teamNameYear ctx (fun season -> season.AutoUpdateFixtures <- model.value)

let delete (club : Club) (teamName, year) (db : Database) =
    club.Teams
    |> List.tryFind (fun t -> (t.ShortName |> Strings.toLower) = (teamName |> toLower))
    |> function 
    | None -> NotFound
    | Some team -> 
        let startDate = DateTime(year, 01, 01)
        let endDate = DateTime(year, 12, 31)
        db.Seasons.Where(fun s -> s.TeamId = team.Id && s.StartDate >= startDate && s.StartDate <= endDate)
        |> Seq.tryHead 
        |> function 
        | None -> NotFound
        | Some season -> 
            db.Seasons.Remove(season) |> ignore
            db.SaveChanges() |> ignore
            OkResult()
