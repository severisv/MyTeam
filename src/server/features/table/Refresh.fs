module MyTeam.Table.Refresh

open MyTeam
open Shared
open System
open System.Net
open Giraffe
open FSharp.Data
open Microsoft.Extensions.Logging

#if !DEBUG
type TableHtml = HtmlProvider<"https://www.fotball.no/fotballdata/turnering/tabell/?fiksId=199029">
#else
type TableHtml = HtmlProvider<"features/table/table.html">
#endif

let run next (ctx: HttpContext) =
    let now = DateTime.Now
    let db = ctx.Database
    let logger = Logger.get ctx.RequestServices

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
    |> List.iter (fun season ->
        try
            let html = TableHtml.Load(season.TableSourceUrl)

            let table =
                html.Html.CssSelect(".a_desktopOnly table")
                |> Seq.skip 1 // Skip the simple table, use the extended table (2nd one)
                |> Seq.head

            let indices =
                {| Plass = 0
                   Lag = 1
                   Poeng = 16
                   Mål = 14
                   V = 11
                   U = 12
                   T = 13 |}

            let table =
                table.CssSelect("tr")
                |> List.skip 2
                |> List.filter (fun row ->
                    // Only process rows with enough columns
                    let cells = row.CssSelect("td")
                    cells.Length > indices.Poeng)
                |> List.map (fun row ->
                    let cells = row.CssSelect("td")

                    ({| Plass = cells.[indices.Plass].InnerText()
                        Lag =
                         cells.[indices.Lag].InnerText()
                         |> WebUtility.HtmlDecode
                        Poeng = cells.[indices.Poeng].InnerText()
                        Mål = cells.[indices.Mål].InnerText()
                        V = cells.[indices.V].InnerText()
                        U = cells.[indices.U].InnerText()
                        T = cells.[indices.T].InnerText() |}))
                |> List.filter (fun row -> row.Plass |> Number.isNumber)
                |> List.map (fun row ->
                    { Team = row.Lag
                      Position = row.Plass |> int
                      Points = row.Poeng |> int
                      GoalsFor =
                        row.Mål
                        |> Strings.split '-'
                        |> List.head
                        |> Strings.trim
                        |> int
                      GoalsAgainst =
                        row.Mål
                        |> Strings.split '-'
                        |> List.last
                        |> Strings.trim
                        |> int
                      Wins = row.V |> int
                      Draws = row.U |> int
                      Losses = row.T |> int }

                )

            season.TableJson <- Json.fableSerialize table
            season.TableUpdated <- now
            db.SaveChanges() |> ignore
        with
        | ex -> logger.LogError(EventId(), ex, sprintf "Klarte ikke scrape tabell. SeasonId: %O  Url: %s" season.Id season.TableSourceUrl))

    text "Ok" next ctx
