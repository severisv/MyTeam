module MyTeam.Seasons.Api

open MyTeam
open System
open Giraffe



let refresh next (ctx: HttpContext)  = 
    let now = DateTime.Now
    let db = ctx.Database

    query {
        for season in db.Seasons do
        where (season.StartDate <= now && season.EndDate >= now && season.AutoUpdateTable)
        select (season)
    }
    |> Seq.filter(fun s -> s.TableSourceUrl |> Strings.hasValue)
    |> Seq.map(fun s -> s.TableSourceUrl)
    |> String.concat ", "
    |> fun res ->

        text res next ctx
    
    // .Where(s => s.StartDate <= now && s.EndDate >= now && s.AutoUpdateTable && !string.IsNullOrWhiteSpace(s.TableSourceUrl)).ToList();
