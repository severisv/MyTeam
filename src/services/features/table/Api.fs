module MyTeam.Table.Api

open System
open MyTeam
open MyTeam.Domain
open MyTeam.Models
open MyTeam.Events
open MyTeam.Shared.Components.Input

let setTitle (club: Club) (teamName, year) (ctx : HttpContext) model =
    let db = ctx.Database
    let (ClubId clubId) = club.Id
    club.Teams
    |> Seq.tryFind (fun t -> (t.ShortName |> Strings.toLower) = (teamName |> toLower))
    |> function
    | None -> NotFound
    | Some team ->
        db.Seasons
        |> Seq.tryFind(fun s -> s.TeamId = team.Id && s.StartDate.Year = year)
        |> function
        | None ->                
            db.Seasons.Add(
                        Models.Domain.Season(
                            StartDate = DateTime(year, 01, 01),
                            EndDate = DateTime(year, 12, 31),
                            Name = model.Value,
                            TeamId = team.Id
                        )
                    ) |> ignore
        | Some s ->
            s.Name <- model.Value
            ()
        
        db.SaveChanges() |> ignore
        OkResult()
            


