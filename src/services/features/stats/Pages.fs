namespace MyTeam

open Giraffe
open Giraffe.GiraffeViewEngine
open MyTeam
open MyTeam.Domain
open System.Linq
open MyTeam.Models.Enums
open MyTeam.Views
open System
open MyTeam.Stats

module StatsPages =  

 
    let index (club: Club) user selectedTeamShortName selectedYear next (ctx: HttpContext) =

        let db = ctx.Database

        let selectedTeam = 
                match selectedTeamShortName with
                | None -> Team  (club.Teams |> List.head)
                | Some s when (s |> toLower) = "samlet" -> All club.Teams
                | Some s -> Team (club.Teams |> List.find (fun t -> t.ShortName |> toLower = (s |> toLower)))
        
        let teamIds = 
            match selectedTeam with
            | Team t -> [t.Id]
            | All teams -> teams |> List.map (fun t -> t.Id)            

        let treningskamp = (Nullable <| int GameType.Treningskamp)
        let years =
            query {
                for gameEvent in db.GameEvents do
                where (teamIds.Contains(gameEvent.Game.TeamId) 
                      && gameEvent.Game.GameType <> treningskamp)
                select (gameEvent.Game.DateTime.Year)
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
                       | Year y -> str y           
            
            sprintf "/statistikk/%s/%s" team year       


        let isSelected url = 
            statsUrl selectedTeam selectedYear = url      

        let getImage = Images.getMember ctx

      
        ([
            main [] [
                block [] [
                    row [] [
                        tabs({ 
                                Items = (club.Teams 
                                        |> List.map (fun team  -> 
                                                            { 
                                                                Text = team.Name
                                                                ShortText = team.ShortName
                                                                Icon = Some <| Icons.team ""
                                                                Url = statsUrl (Team team) selectedYear 
                                                            }
                                                    )) @ [
                                                            {                                                              
                                                                Text = "Samlet"
                                                                ShortText = "Samlet"
                                                                Icon = None
                                                                Url = statsUrl (All club.Teams) selectedYear
                                                            }
                                                        ]                           
                                IsSelected = isSelected
                           })                        
                   
                        navListMobile
                            ({ 
                                Header = "Sesonger"
                                Items = years |> List.map (fun year  -> { Text = str year; Url = statsUrl selectedTeam (Year year) }                                                                   )  
                                Footer = Some <| { Text = "Total"; Url = statsUrl selectedTeam AllYears }                                                               
                                IsSelected = isSelected                                                               
                           })
                    ]
                ]     
                block [] [
                    table [_class "stats-table"] (
                        {
                            Type = Image
                            Columns = 
                                [
                                    { Value = [Str "Spiller"]; Align = Left }
                                    { Value = [Node <| Icons.player "Kamper"]; Align = Center }
                                    { Value = [Node <| Icons.goal "Mål"]; Align = Center }
                                    { Value = [Node <| Icons.assist "Assists"]; Align = Center }
                                    { Value = [Node <| Icons.yellowCard "Gule Kort"]; Align = Center }
                                    { Value = [Node <| Icons.redCard "Røde kort"]; Align = Center }
                                ]
                            Rows = 
                                (stats |> List.map (fun player ->
                                                    [
                                                        Node(span [] [
                                                                    img [_src <| getImage player.Image player.FacebookId (fun o -> { o with Height = Some 50; Width = Some 50 })] 
                                                                    whitespace 
                                                                    a [_href <| sprintf "/spillere/vis/%s" player.UrlName] 
                                                                        [
                                                                            encodedText player.Name
                                                                        ]                                                       
                                                                ] 
                                                                )                                    
                                                                                                                            
                                                        Number player.Games
                                                        Number player.Goals
                                                        Number player.Assists
                                                        Number player.YellowCards
                                                        Number player.RedCards
                                                    ]
                                                    )
                                )
                        }
                    ) 
                ]
            ]
            (years.Length > 0 =?
                (
                sidebar [] [
                    block [] [
                        navList ({ 
                                    Header = "Sesonger"
                                    Items = years |> List.map (fun year  -> { Text = str year; Url = statsUrl selectedTeam (Year year) }                                                                   )  
                                    Footer = Some <| { Text = "Total"; Url = statsUrl selectedTeam AllYears }                                                               
                                    IsSelected = isSelected                                                               
                               })
                    ]
                ]                                   
                , emptyText))        
        ] 
        |> layout club user (fun o -> { o with Title = "Statistikk"}) ctx
        |> htmlView) next ctx