namespace MyTeam.Table

open Giraffe.GiraffeViewEngine
open MyTeam
open Shared
open Shared.Domain
open Shared.Domain.Members
open MyTeam.Views
open Shared.Components
open Shared.Features.Table.Table
open System
open MyTeam.Views.BaseComponents


module Table =   
    let view (club: Club) user selectedTeamShortName selectedYear (ctx: HttpContext) =

        let db = ctx.Database

        match selectedTeamShortName with
        | None -> club.Teams |> List.tryHead
        | Some s -> club.Teams |> List.tryFind (fun t -> t.ShortName |> toLower = (s |> toLower))
        |> function
        | None -> NotFound
        | Some selectedTeam ->
        
            let years = Queries.getYears db selectedTeam.Id

            let tableUrl (team: Team) year = 
                        sprintf "/tabell/%s/%i" team.ShortName year       
      
            let selectedYear = 
                match selectedYear with
                | None -> years |> List.tryHead
                | Some y when isNumber y -> Number.parse y |> Some
                | _ -> None 
                |> Option.defaultValue DateTime.Now.Year    
                
            
            let t = Queries.getTable db selectedTeam.Id selectedYear 
          
            let isSelected url = 
                tableUrl selectedTeam selectedYear = url      

            [
                mtMain [] [
                    block [] [                                
                        tabs [_class "team-nav"] 
                            (club.Teams |> List.map (fun team  -> 
                                                { Text = team.Name
                                                  ShortText = team.ShortName
                                                  Icon = Some <| !!(Icons.team "")
                                                  Url = tableUrl team selectedYear }
                                        ))                           
                            isSelected            
                   
                        navListMobile
                            ({ Items = years |> List.map (fun year  -> { Text = string year; Url = tableUrl selectedTeam year}                                                                   )  
                               Footer = None                                                               
                               IsSelected = isSelected })
                        hr []
                        t => fun t ->
                                div [] [
                                    (if (user |> Option.map (fun (user: User) -> user.IsInRole [Role.Admin])
                                              |> Option.defaultValue false) then
                                        Client.viewOld editView 
                                                    { Title = t.Title
                                                      Team = selectedTeam.ShortName
                                                      Year = selectedYear
                                                      AutoUpdateTable = t.AutoUpdate
                                                      SourceUrl = t.SourceUrl
                                                      AutoUpdateFixtures = t.AutoUpdateFixtures
                                                      FixtureSourceUrl = t.FixtureSourceUrl }
                                    else 
                                        h2 [] [!!(Icons.trophy ""); whitespace; encodedText t.Title])
                                    br []
                                    table [Striped; TableProperty.Attribute <| _class "table-table"] 
                                                [
                                                    col [NoSort; Align Center; Attr <| _class "hidden-xxs"] []
                                                    col [NoSort] [encodedText "Lag"]
                                                    col [NoSort; Align Center] [encodedText "Kamper"]
                                                    col [NoSort; Align Center; Attr <| _class "hidden-sm hidden-xs"] [encodedText "Seier"]
                                                    col [NoSort; Align Center; Attr <| _class "hidden-sm hidden-xs"] [encodedText "Uavgjort"]
                                                    col [NoSort; Align Center; Attr <| _class "hidden-sm hidden-xs"] [encodedText "Tap"]
                                                    col [NoSort; Align Center; Attr <| _class "hidden-xs"] [encodedText "Målforskjell"]
                                                    col [NoSort; Align Center] [encodedText "Poeng"]
                                                ]
                                                (t.Rows 
                                                 |> List.map (fun r ->
                                                                tableRow [_class (r.Team.Contains(club.Name.Split(' ').[0])  =? ("team-primary", ""))] [
                                                                    number r.Position
                                                                    encodedText r.Team
                                                                    number r.Games
                                                                    number r.Wins
                                                                    number r.Draws
                                                                    number r.Losses
                                                                    encodedText r.GoalDifference
                                                                    number r.Points
                                                                ]
                                                            )
                                                ) 
                                    (selectedYear = DateTime.Now.Year =? ( 
                                        span [_class "subtle ft-sm"] 
                                             [encodedText <| "Sist oppdatert: " + t.UpdatedDate.ToString("d MMM kl HH:mm") ], emptyText))
                                ]
                       ]
                ]
                sidebar [] [
                    user =>
                        fun user ->
                            (if user.IsInRole [Role.Admin] then
                                block [] [
                                        ul [_class "nav nav-list"] [ 
                                            li [_class "nav-header"] [encodedText "Admin"]
                                            li [] [Client.viewOld createView { Team = selectedTeam.ShortName }] 
                                        ]
                                ]       
                            else empty)                                                
                                    
                    (if years.Length > 0 then                                
                        block [] [
                            navList ({ Header = "Sesonger"
                                       Items = years |> List.map (fun year  -> { Text = [encodedText <| string year] 
                                                                                 Url = tableUrl selectedTeam year }                                                                   )  
                                       Footer = None                                                               
                                       IsSelected = isSelected })
                        ]                                                             
                        else emptyText)        
                    ]
            ] 
            |> layout club user (fun o -> { o with Title = "Tabell" }) ctx
            |> OkResult
