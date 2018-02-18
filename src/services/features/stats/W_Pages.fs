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

    let sidebar attributes children = div ([_class "mt-sidebar"] @ attributes) children 
    let block attributes children = div ([_class "mt-container"] @ attributes) children 


    type NavItem = {
        Text: string
        Url: string
    }

    type NavList = {
        Header: string
        Items: NavItem list
        Footer: NavItem option
        IsSelected: (string -> bool)
    }

    let navList model =
        ul [_class "nav nav-list"]
            (
            [ li [_class "nav-header"] [encodedText model.Header] ] @   
            (model.Items |> List.map (fun item  ->
                                li [] [
                                    a [_href <| item.Url;_class (model.IsSelected <| item.Url =? ("active", "")) ] [
                                        encodedText item.Text
                                    ]
                                ] 
                               )) @
            match model.Footer with 
            | Some footer ->   
                [ 
                li [] [hr [] ]
                li [] [a [_href footer.Url;_class (model.IsSelected footer.Url =? ("active", ""))] [
                             encodedText footer.Text
                      ]
                    ]
                ]      
            | None -> [] 
            )  
    
    let navListMobile model =
        div [_class "nav-list--mobile"] [
                select [_class "linkSelect form-control pull-right hidden-md hidden-lg"]  
                    (
                    (model.Items |> List.map (fun item ->
                                        option [_value item.Url; model.IsSelected item.Url =? (_selected, _empty) ] [
                                            encodedText item.Text
                                        ] 
                                       )) @
                    match model.Footer with
                    | Some footer ->                    
                        [ 
                            option [_value footer.Url; model.IsSelected footer.Url =? (_selected, _empty)] [
                                encodedText footer.Text
                            ]
                        ]     
                    | None -> [])     
            ]

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


        let isSelectedTeam team = 
            selectedTeam = team

        let isSelectedYear url = 
            statsUrl selectedTeam selectedYear = url      

        let getImage = Images.getMember ctx

      
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
                            navListMobile
                                ({ 
                                    Header = "Sesonger"
                                    Items = years |> List.map (fun year  -> { Text = str year; Url = statsUrl selectedTeam (Year year) }                                                                   )  
                                    Footer = Some <| { Text = "Total"; Url = statsUrl selectedTeam AllYears }                                                               
                                    IsSelected = isSelectedYear                                                               
                               })
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
                sidebar [] [
                    block [] [
                        navList ({ 
                                    Header = "Sesonger"
                                    Items = years |> List.map (fun year  -> { Text = str year; Url = statsUrl selectedTeam (Year year) }                                                                   )  
                                    Footer = Some <| { Text = "Total"; Url = statsUrl selectedTeam AllYears }                                                               
                                    IsSelected = isSelectedYear                                                               
                               })
                    ]
                ]                                   
                , emptyText))        
        ] 
        |> layout club user (fun o -> { o with Title = "Statistikk"}) ctx
        |> htmlView) next ctx