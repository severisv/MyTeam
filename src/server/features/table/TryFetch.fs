module MyTeam.Table.TryFetch

open MyTeam
open Shared
open System
open Giraffe
open FSharp.Data
open Microsoft.Extensions.Logging
open System.Net.Http

#if !DEBUG
type TableHtml = HtmlProvider<"https://www.fotball.no/fotballdata/turnering/tabell/?fiksId=158443">
#else
type TableHtml = HtmlProvider<"features/table/table.html">
#endif

let run next (ctx: HttpContext) =
    let now = DateTime.Now
    let db = ctx.Database
    let logger = Logger.get ctx.RequestServices

    let result =
        query {
            for season in db.Seasons do
                where (
                    season.StartDate <= now
                    && season.EndDate >= now
                    && season.AutoUpdateTable
                    && season.TableSourceUrl <> null
                )

                select (season)
        }
        |> Seq.toList
        |> List.map (fun season ->

            use httpClient = new HttpClient()

            let content =
                httpClient
                    .GetStringAsync(
                        season.TableSourceUrl
                    )
                    .Result

            let html = TableHtml.Parse(content)

            let table =
                html.Tables.Table1.Rows
                |> Array.filter (fun row -> row.Plass |> Number.isNumber)
                |> Array.map (fun row ->
                    { Team = row.Lag
                      Position = row.Plass |> int
                      Points = row.Poeng
                      GoalsFor =
                        row.``Total - Mål``
                        |> Strings.split '-'
                        |> List.head
                        |> Strings.trim
                        |> int
                      GoalsAgainst =
                        row.``Total - Mål``
                        |> Strings.split '-'
                        |> List.last
                        |> Strings.trim
                        |> int
                      Wins = row.``Total - V``
                      Draws = row.``Total - U``
                      Losses = row.``Total - T`` }

                )

            table)

    json result next ctx
