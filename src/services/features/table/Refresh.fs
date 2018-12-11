module MyTeam.Table.Refresh

open MyTeam
open System
open Giraffe
open FSharp.Data
open MyTeam.Domain.Table
open Microsoft.Extensions.Logging

type Table = HtmlProvider<"https://www.fotball.no/fotballdata/turnering/tabell/?fiksId=158443">

let run next (ctx: HttpContext)  = 
    let now = DateTime.Now
    let db = ctx.Database
    let logger = Logger.get ctx

    query {
        for season in db.Seasons do
        where (season.StartDate <= now && 
                season.EndDate >= now && 
                season.AutoUpdateTable && 
                season.TableSourceUrl <> null)
        select (season)
    }
    |> Seq.toList
    |> List.iter (fun season ->
        try
            let html = Table.Load(season.TableSourceUrl)
            let table = html.Tables.Table1.Rows
                        |> Array.filter (fun row -> row.Plass |> Number.isNumber)
                        |> Array.map (fun row -> 
                                            {
                                                Team = row.Lag
                                                Position = row.Plass |> int
                                                Points = row.Poeng
                                                GoalsFor = row.``Total - Mål`` |> Strings.split [|'-'|] |> Array.head |> Strings.trim |> int
                                                GoalsAgainst = row.``Total - Mål`` |> Strings.split [|'-'|] |> Array.rev |> Array.head |> Strings.trim |> int
                                                Wins = row.``Total - V``
                                                Draws = row.``Total - U``
                                                Losses = row.``Total - T``
                                            }

                            )   
            season.TableJson <- Json.fableSerialize table 
            season.TableUpdated <- now 
            db.SaveChanges() |> ignore
        with
            | ex ->  logger.LogError(EventId(), ex, 
                                sprintf "Klarte ikke scrape tabell. SeasonId: %O  Url: %s" 
                                            season.Id
                                            season.TableSourceUrl
                                )                        
    )

    text "Ok" next ctx
    

