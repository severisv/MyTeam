namespace MyTeam.Stats

open Giraffe.GiraffeViewEngine
open MyTeam
open Shared
open Shared.Domain
open System.Linq
open MyTeam.Models.Enums
open MyTeam.Views
open System
open MyTeam.Stats
open Shared.Components
open Shared.Components.Nav
open MyTeam.Views.BaseComponents
open TableModule

module Pages =   
    let index (club: Club) user selectedTeamShortName selectedYear (ctx: HttpContext) =

        let db = ctx.Database

        match selectedTeamShortName with
        | None -> club.Teams |> List.tryHead |> Option.map(Team)
        | Some s when (s |> toLower) = "samlet" -> Elleven (club.Teams |> List.filter (fun t -> t.LeagueType = Ellever)) |> Some
        | Some s when (s |> toLower) = "ellever" -> Elleven (club.Teams |> List.filter (fun t -> t.LeagueType = Ellever)) |> Some
        | Some s when (s |> toLower) = "syver" -> Seven (club.Teams |> List.filter (fun t -> t.LeagueType = Syver)) |> Some
        | Some s -> club.Teams 
                    |> List.tryFind (fun t -> t.ShortName |> toLower = (s |> toLower))
                    |> Option.map Team
        |> function
        | None -> NotFound
        | Some selectedTeam ->
        
            let teamIds = 
                match selectedTeam with
                | Team t -> [t.Id]
                | Seven teams -> teams |> List.map (fun t -> t.Id)            
                | Elleven teams -> teams |> List.map (fun t -> t.Id)            

            let treningskamp = (Nullable <| int GameType.Treningskamp)
            let years =
                query {
                    for game in db.Games do
                    where (teamIds.Contains(game.TeamId.Value) 
                          && game.GameType <> treningskamp)
                    select (game.DateTime.Year)
                    distinct
                }
                |> Seq.toList
                |> List.sortDescending
      
            let selectedYear = 
                match selectedYear with
                | None -> Year (years |> List.head)
                | Some y when y = "total" -> AllYears
                | Some y when y |> isNumber -> Year <| Number.parse y
                | Some y -> failwithf "Valgt år kan ikke være %s" y

            let stats = StatsQueries.get db selectedTeam selectedYear 
                           
            let statsUrl team year = 
                let team = match team with 
                           | Seven _ -> "syver"
                           | Elleven _ -> "ellever"
                           | Team t -> t.ShortName 
                let year = match year with
                           | AllYears _ -> "total"
                           | Year y -> string y           
                
                sprintf "/statistikk/%s/%s" team year       

            let isSelected url = 
                statsUrl selectedTeam selectedYear = url      

            let getImage = Images.getMember ctx
            
            let leagueTypes =
                club.Teams
                |> List.map (fun team -> team.LeagueType)
                |> List.distinct
          
            [
                mtMain [] [
                    block [_class "stats"] [
                        !!(Tabs.tabs [Fable.React.Props.Class "team-nav stats-nav"] 
                                 ((club.Teams 
                                 |> List.map (fun team  -> 
                                                    {   Text = team.ShortName
                                                        ShortText = team.ShortName
                                                        Icon = Some <| Icons.team ""
                                                        Url = statsUrl (Team team) selectedYear  }
                                            )) @ (leagueTypes
                                                |> List.map (fun leagueType ->
                                                               let text = if leagueTypes.Length > 1 then (match leagueType with | Syver -> "7'er" | Ellever -> "11'er")
                                                                          else "Samlet"
                                                               { Text = sprintf "Samlet %s" text
                                                                 ShortText = text
                                                                 Icon = None
                                                                 Url = statsUrl (match leagueType with
                                                                                 | Syver -> Seven []
                                                                                 | Ellever -> Elleven []
                                                                                 ) selectedYear })))                           
                                isSelected)
                        !!(navListMobile
                            {  Items = years |> List.map (fun year  -> { Text = string year; Url = statsUrl selectedTeam (Year year) }                                                                   )  
                               Footer = Some <| { Text = "Total"; Url = statsUrl selectedTeam AllYears }                                                               
                               IsSelected = isSelected })
                        hr []
                        br []                     
                        table [Striped;TableProperty.Attribute <| _class "stats-table"] 
                                    [
                                        col [CellType Image; NoSort] []
                                        col [] [encodedText "Spiller"]
                                        col [Align Center] [!!(Icons.player "Kamper")]
                                        col [Align Center] [!!(Icons.goal "Mål")]
                                        col [Align Center] [!!(Icons.assist "Assists")]
                                        col [Align Center] [!!(Icons.yellowCard "Gule Kort")]
                                        col [Align Center] [!!(Icons.redCard "Røde kort")]
                                    ]
                                    (stats |> List.map (fun player ->
                                                        let playerLink = a [_href <| sprintf "/spillere/vis/%s" player.UrlName; _title player.Name]
                                                        tableRow [] [
                                                            playerLink [ img [_src <| getImage (fun o -> { o with Height = Some 50; Width = Some 50 }) player.Image player.FacebookId ] ]
                                                            playerLink [encodedText player.Name]                                                       
                                                            number player.Games
                                                            number player.Goals
                                                            number player.Assists
                                                            number player.YellowCards
                                                            number player.RedCards
                                                        ]
                                                        )
                                    ) 
                    ]
                ]
                (years.Length > 0 =?
                    (
                    sidebar [] [
                        block [] [
                            !!(navList {Header = "Sesonger"
                                        Items = years |> List.map (fun year  -> { Text = [Fable.React.Helpers.str <| string year]
                                                                                  Url = statsUrl selectedTeam (Year year) }                                                                   )  
                                        Footer = Some <| { Text = [Fable.React.Helpers.str "Total"]; Url = statsUrl selectedTeam AllYears }                                                               
                                        IsSelected = isSelected })]
                    ], emptyText))        
            ] 
            |> layout club user (fun o -> { o with Title = "Statistikk"}) ctx
            |> OkResult