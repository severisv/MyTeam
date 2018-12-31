namespace MyTeam.Table

open Giraffe.GiraffeViewEngine
open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Members
open MyTeam.Views
open MyTeam.Shared.Components
open Shared.Features.Table.Table
open System

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
      
            match selectedYear with
            | None -> years |> List.tryHead
            | Some y when isNumber y -> Number.parse y |> Some
            | _ -> None 
            |> function
            | Some selectedYear when years |> List.exists (fun y -> y = selectedYear) ->
                Queries.getTable db selectedTeam.Id selectedYear 
                |> function
                | Some t ->               

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
                                    ({ Header = "Sesonger"
                                       Items = years |> List.map (fun year  -> { Text = string year; Url = tableUrl selectedTeam year}                                                                   )  
                                       Footer = None                                                               
                                       IsSelected = isSelected })
                                hr []
                                (if (user |> Option.map (fun (user: Users.User) -> user.IsInRole [Role.Admin])
                                          |> Option.defaultValue false) then
                                    Client.view editView 
                                                { Title = t.Title
                                                  Team = selectedTeam.ShortName
                                                  Year = selectedYear
                                                  AutoUpdateTable = t.AutoUpdate
                                                  SourceUrl = t.SourceUrl }
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
                        sidebar [] [
                            user =>
                                fun user ->
                                    (if user.IsInRole [Role.Admin] then
                                        block [] [
                                                ul [_class "nav nav-list"] [ 
                                                    li [_class "nav-header"] [encodedText "Admin"]
                                                    li [] [Client.view createView 
                                                                        { Team = selectedTeam.ShortName }] 
                                                ]
                                        ]       
                                    else empty)                                                
                                            
                            (if years.Length > 0 then                                
                                block [] [
                                    navList ({ 
                                                Header = "Sesonger"
                                                Items = years |> List.map (fun year  -> { Text = [encodedText <| string year]; Url = tableUrl selectedTeam year }                                                                   )  
                                                Footer = None                                                               
                                                IsSelected = isSelected                                                               
                                           })
                                ]                                                             
                                else emptyText)        
                            ]
                    ] 
                    |> layout club user (fun o -> { o with Title = "Tabell" }) ctx
                    |> OkResult

                | None -> 
                    years
                    |> List.sortByDescending id
                    |> List.tryHead
                    |> function
                    | Some y -> Redirect <| tableUrl selectedTeam y
                    | None -> NotFound        
            | _ -> 
                years
                |> List.sortByDescending id
                |> List.tryHead
                |> function
                | Some y -> Redirect <| tableUrl selectedTeam y
                | None -> NotFound        