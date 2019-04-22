namespace MyTeam.Stats

open Giraffe
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
open MyTeam.Views.BaseComponents

module Pages =   
    let index (club: Club) user selectedTeamShortName selectedYear (ctx: HttpContext) =

        let db = ctx.Database

        match selectedTeamShortName with
        | None -> club.Teams |> List.tryHead |> Option.map(Team)
        | Some s when (s |> toLower) = "samlet" -> All club.Teams |> Some
        | Some s -> club.Teams 
                    |> List.tryFind (fun t -> t.ShortName |> toLower = (s |> toLower))
                    |> Option.map(Team)
        |> function
        | None -> NotFound
        | Some selectedTeam ->
        
            let teamIds = 
                match selectedTeam with
                | Team t -> [t.Id]
                | All teams -> teams |> List.map (fun t -> t.Id)            

            let treningskamp = (Nullable <| int GameType.Treningskamp)
            let years =
                query {
                    for game in db.Games do
                    where (teamIds.Contains(game.TeamId) 
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
                           | All _ -> "samlet"
                           | Team t -> t.ShortName 
                let year = match year with
                           | AllYears _ -> "total"
                           | Year y -> string y           
                
                sprintf "/statistikk/%s/%s" team year       


            let isSelected url = 
                statsUrl selectedTeam selectedYear = url      

            let getImage = Images.getMember ctx
          
            [
                mtMain [] [
                    block [] [
                        tabs [_class "team-nav"] 
                             ((club.Teams 
                             |> List.map (fun team  -> 
                                                { 
                                                    Text = team.Name
                                                    ShortText = team.ShortName
                                                    Icon = Some <| !!(Icons.team "")
                                                    Url = statsUrl (Team team) selectedYear 
                                                }
                                        )) @ [
                                                {                                                              
                                                    Text = "Samlet"
                                                    ShortText = "Samlet"
                                                    Icon = None
                                                    Url = statsUrl (All club.Teams) selectedYear
                                                }
                                            ])                           
                            isSelected
                            
                        navListMobile
                            ({ 
                                Header = "Sesonger"
                                Items = years |> List.map (fun year  -> { Text = string year; Url = statsUrl selectedTeam (Year year) }                                                                   )  
                                Footer = Some <| { Text = "Total"; Url = statsUrl selectedTeam AllYears }                                                               
                                IsSelected = isSelected                                                               
                           })
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
                            navList ({ 
                                        Header = "Sesonger"
                                        Items = years |> List.map (fun year  -> { Text = [encodedText <| string year]; Url = statsUrl selectedTeam (Year year) }                                                                   )  
                                        Footer = Some <| { Text = [encodedText "Total"]; Url = statsUrl selectedTeam AllYears }                                                               
                                        IsSelected = isSelected                                                               
                                   })
                        ]
                    ]                                   
                    , emptyText))        
            ] 
            |> layout club user (fun o -> { o with Title = "Statistikk"}) ctx
            |> OkResult