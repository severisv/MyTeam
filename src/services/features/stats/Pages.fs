namespace MyTeam

open Giraffe
open Giraffe.GiraffeViewEngine
open MyTeam
open MyTeam.Domain
open MyTeam.Authorization
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

        let getImage = Images.getMember ctx

        let isSelectedTeam team = 
            selectedTeam = team

        let isSelectedYear year = 
            selectedYear = year        

        ([
            div [_class "mt-main"] [
                div [_class "mt-container"] [
                    div [_class "row"] [
                        club.Teams.Length > 1 =?                        
                            (div [_class "stats-teamNav"] [
                                ul [_class "nav nav-pills mt-justified"] 
                                    ((club.Teams 
                                        |> List.map (fun t -> 
                                                        li [_class (isSelectedTeam <| Team t =? ("active", ""))] [
                                                            a [_href <| statsUrl (Team t) selectedYear] [
                                                                i [_class "hidden-xs flaticon-football43"] [] 
                                                                span [_class "hidden-xs"] [whitespace;encodedText t.Name] 
                                                                span [_class "visible-xs"] [encodedText t.ShortName]
                                                            ]
                                                        ]                                    
                                    )) @ [ 
                                            li [_class (isSelectedTeam <| All club.Teams =? ("active", ""))] [
                                                a [_href <| statsUrl (All club.Teams) selectedYear ] [encodedText "Samlet" ]
                                            ]
                                        ]
                                    )
                                ]                                                                                                           
                            , emptyText)

                        (years.Length > 0 =?
                            (
                            div [_class "stats-yearNav--mobile"] [
                                select [_class "linkSelect form-control pull-right hidden-md hidden-lg"]  
                                    (List.append 
                                    (years |> List.map (fun y  ->
                                                        option [_value <| statsUrl selectedTeam (Year y); isSelectedYear <| Year y =? (_selected, _empty) ] [
                                                            encodedText <| str y
                                                        ] 
                                                       ))
                                    [ 
                                        option [_value <| statsUrl selectedTeam AllYears; isSelectedYear <| AllYears =? (_selected, _empty)] [
                                            encodedText "Total"
                                        ]
                                    ]      
                                    )                                        
                                
                            ]
                            , emptyText))
                    ]
                ]     
                div [_class "mt-container"] [
                    table [_class "table tablesorter stats-table attendance-table"] [
                        thead [] [
                            tr [] [
                                    th [] [encodedText "Spiller"]
                                    th [_class "score"] [Icons.player "Kamper"] 
                                    th [_class "score"] [Icons.goal "Mål"]
                                    th [_class "score"] [Icons.assist "Assists"]
                                    th [_class "score"] [Icons.yellowCard "Gule Kort"]
                                    th [_class "score"] [Icons.redCard "Røde kort" ]
                            ]
                        ]
                        tbody [] 
                                (stats |> List.map(fun player ->
                                                    tr [] [
                                                            td [_class "attendance-player-image"] [
                                                                span [_class "attendance-playerImageContainer"] [
                                                                    img [_src <| getImage player.Image player.FacebookId (fun o -> { o with Height = Some 50; Width = Some 50 })] 
                                                                    whitespace                                                        
                                                                ] 
                                                                a [_href <| sprintf "/spillere/vis/%s" player.UrlName] [
                                                                    encodedText player.Name
                                                                ]
                                                            ]                                                
                                                            td [_class "score"] [encodedText <| str player.Games]
                                                            td [_class "score"] [encodedText <| str player.Goals]
                                                            td [_class "score"] [encodedText <| str player.Assists]
                                                            td [_class "score"] [encodedText <| str player.YellowCards]
                                                            td [_class "score"] [encodedText <| str player.RedCards]
                                                        ]
                                        )
                                )
                    ]
                ]
            ]
            (years.Length > 0 =?
                            (
                            div [_class "mt-sidebar"] [
                                div [_class "mt-container"] [
                                    ul [_class "nav nav-list"]
                                        (
                                        [ li [_class "nav-header"] [encodedText "Sesonger"] ] @   
                                        (years |> List.map (fun year  ->
                                                            li [] [
                                                                a [_href <| statsUrl selectedTeam (Year year);_class (isSelectedYear <| Year year =? ("active", "")) ] [
                                                                    encodedText <| str year
                                                                ]
                                                            ] 
                                                           )) @
                                        [ 
                                            li [] [hr [] ]
                                            li [] [a [_href <| statsUrl selectedTeam AllYears;_class (isSelectedYear AllYears =? ("active", ""))] [
                                                         encodedText "Total"
                                                  ]
                                            ]
                                        ]      
                                        )  
                                                                         
                                ]
                            ]
                            , emptyText))        
        ] 
        |> layout club user (fun o -> { o with Title = "Statistikk"}) ctx
        |> renderHtml) next ctx