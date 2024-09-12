module MyTeam.Games.Refresh

open MyTeam
open Shared
open Shared.Domain
open System
open Giraffe
open FSharp.Data
open Microsoft.Extensions.Logging
open System.Collections.Generic

#if !DEBUG
type GamesHtml = HtmlProvider<"https://www.fotball.no/fotballdata/turnering/terminliste/?fiksId=158443">
#else
type GamesHtml = HtmlProvider<"features/game/games.html">
#endif


let run next (ctx: HttpContext)  = 
    let now = DateTime.Now
    let db = ctx.Database
    let logger = Logger.get ctx.RequestServices

    query {
        for season in db.Seasons do
        where (season.StartDate <= now && 
                season.EndDate >= now && 
                season.AutoUpdateFixtures && 
                season.FixturesSourceUrl <> null)
        select (season, season.Team)
    }
    |> Seq.toList
    |> Seq.map (fun (season, team) ->
        try

            let isLike a b =
                b 
                |> (Strings.toLower >> Strings.removeAllWhitespaces) 
                |> Strings.contains <| (Strings.removeAllWhitespaces >> Strings.toLower) a

            let isCurrentTeam =
                isLike team.Name

            let existingGames = 
                let serieKamp = (Events.gameTypeToInt GameType.Seriekamp |> Nullable)
                query {
                    for game in db.Games do
                    where (
                           game.TeamId = Nullable season.TeamId &&
                           game.DateTime >= season.StartDate &&
                           game.DateTime <= season.EndDate &&
                           game.GameType = serieKamp
                    )
                    select(game)
                }
                |> Seq.toList

            let html = GamesHtml.Load(season.FixturesSourceUrl)

            let table = html.Html.CssSelect("table") |> Seq.head

            let headers = table.CssSelect("th") |> Seq.map (fun th -> th.InnerText().ToLowerInvariant()) |> Seq.toList

            let indices = {|
                Hjemmelag = headers |> List.findIndex (fun h -> h.Contains("hjemmelag"))
                Bortelag = headers |> List.findIndex (fun h -> h.Contains("bortelag"))
                Dato = headers |> List.findIndex (fun h -> h.Contains("dato"))
                Tid = headers |> List.findIndex (fun h -> h.Contains("tid"))
                Bane = headers |> List.findIndex (fun h -> h.Contains("bane"))
            |}



            table.CssSelect("tr")
            |> List.skip 1
            |> List.map (fun row -> 

                ({|
                Hjemmelag = (row.CssSelect("td").[indices.Hjemmelag]).InnerText() 
                Bortelag = (row.CssSelect("td").[indices.Bortelag]).InnerText()
                Dato = (row.CssSelect("td").[indices.Dato]).InnerText()
                Tid = (row.CssSelect("td").[indices.Tid]).InnerText()
                Bane = (row.CssSelect("td").[indices.Bane]).InnerText()
            |}))
            |> List.filter (fun row ->                                                           
                                row.Hjemmelag |> isCurrentTeam || 
                                row.Bortelag |> isCurrentTeam)
            |> List.iter (fun row -> 
                                let isHomeTeam = isCurrentTeam row.Hjemmelag
                                let opponent = if isHomeTeam then row.Bortelag else row.Hjemmelag 
                                               |> Strings.removeDoubleWhitespaces  
                                
                                let date =
                                    try row.Dato.Split "." |> fun v -> DateTime(int v.[2], int v.[1], int v.[0])
                                    with _ -> failwith $"Klarte ikke parse dato for kamp: {row}. Season: {season}"
                                
                                if date < season.StartDate || date > season.EndDate then
                                    logger.LogWarning $"Prøver å legge til kamp som ikke er for gjeldende sesong. SeasonId {season.Id} Rad: {row}"
                                    ()

                                else
                                    let game = 
                                        existingGames 
                                        |> List.tryFind (fun game -> (game.Opponent |> isLike opponent) && game.IsHomeTeam = isHomeTeam)
                                        |> function
                                        | Some existingGame -> 
                                            db.Events.Attach(existingGame) |> ignore
                                            existingGame
                                        | None ->
                                                                             
                                            let et = List<Models.Domain.EventTeam>()
                                            et.Add(Models.Domain.EventTeam( TeamId = season.TeamId ))

                                            let g = 
                                                Models.Domain.Event(
                                                    IsHomeTeam = isHomeTeam,
                                                    Opponent = opponent,
                                                    TeamId = Nullable season.TeamId,
                                                    EventTeams = et,
                                                    Type = Events.eventTypeToInt EventType.Kamp,
                                                    ClubId = team.ClubId,
                                                    GameType = ((Events.gameTypeToInt GameType.Seriekamp) |> Nullable)
                                                )
                                            db.Events.Add(g) |> ignore
                                            g                                            

                                    let time = row.Tid 
                                               |> string 
                                               |> Strings.trim
                                               |> Strings.split '.'
                                               |> List.map float
                                               |> function 
                                               | [hr; minute] -> TimeSpan.FromHours(hr).Add(TimeSpan.FromMinutes minute)
                                               | r -> failwithf $"Klarte ikke parse tid: {r}"
                                                                                   
                                    game.DateTime <- (date.Date.Add time)
                                    game.Location <- row.Bane |> Strings.removeDoubleWhitespaces                           
                        )

            season.FixturesUpdated <- Nullable now 
            db.SaveChanges() |> ignore    
            Ok ()
            
        with
            | ex ->  
                logger.LogError(EventId(), ex, 
                                $"Klarte ikke scrape kamper. SeasonId: {season.Id}  Url: %s{season.FixturesSourceUrl}"
                                )                        
                Error ()                            
    )
    |> fun res ->
        let success = res |> Seq.filter (function | Ok _ -> true | Error _ -> false)
                          |> Seq.toList                                                
                                            
        text $"Parset %i{success.Length} sesonger" next ctx
    

