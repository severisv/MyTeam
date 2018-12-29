module MyTeam.Table.Api

open MyTeam
open MyTeam.Domain
open MyTeam.Events
open MyTeam.Models
open MyTeam.Shared.Components.Input
open System

let internal update (club : Club) (teamName, year) (ctx : HttpContext) updateFn =
    let db = ctx.Database
    let (ClubId clubId) = club.Id
    club.Teams
    |> Seq.tryFind (fun t -> (t.ShortName |> Strings.toLower) = (teamName |> toLower))
    |> function 
    | None -> NotFound
    | Some team -> 
        db.Seasons
        |> Seq.tryFind (fun s -> s.TeamId = team.Id && s.StartDate.Year = year)
        |> function 
        | None -> 
            let season =
                Models.Domain.Season
                    (StartDate = DateTime(year, 01, 01), EndDate = DateTime(year, 12, 31), Name = "", 
                     TeamId = team.Id, TableSourceUrl = "")
            db.Seasons.Add(season) |> ignore
            season
        | Some s -> s
        |> fun season -> 
            updateFn season
            db.SaveChanges() |> ignore
            OkResult()

let setTitle club teamNameYear ctx model =
    update club teamNameYear ctx (fun season -> season.Name <- model.Value)

let setSourceUrl club teamNameYear ctx model =
    update club teamNameYear ctx (fun season -> season.TableSourceUrl <- model.Value)

let setAutoUpdate club teamNameYear ctx (model: CheckboxPayload) =
    update club teamNameYear ctx (fun season -> season.AutoUpdateTable <- model.value)
    