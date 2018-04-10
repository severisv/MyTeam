namespace MyTeam.Table

open Giraffe
open Giraffe.GiraffeViewEngine
open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Members
open System.Linq
open MyTeam.Models.Enums
open MyTeam.Views
open System

module Pages =   
    let index (club: Club) user selectedTeamShortName selectedYear (ctx: HttpContext) =

        let db = ctx.Database

        match selectedTeamShortName with
        | None -> club.Teams |> List.tryHead
        | Some s -> club.Teams |> List.tryFind (fun t -> t.ShortName |> toLower = (s |> toLower))
        |> function
        | None -> Error NotFound
        | Some selectedTeam ->
        
            let years = Queries.getYears db selectedTeam.Id
      
            match selectedYear with
            | None -> years |> List.tryHead
            | Some y when isNumber y -> Number.parse y |> Some
            | _ -> None 
            |> function
            | Some selectedYear when years |> List.exists (fun y -> y = selectedYear) ->
                Queries.getTable db selectedTeam.Id selectedYear 
                |> function
                | Some t ->               

                    let tableUrl (team: Team) year = 
                        sprintf "/tabell/%s/%i" team.ShortName year       

                    let isSelected url = 
                        tableUrl selectedTeam selectedYear = url      
                  
                    [
                        main [] [
                            block [] [
                                tabs [_class "team-nav"] 
                                    (club.Teams |> List.map (fun team  -> 
                                                        { 
                                                            Text = team.Name
                                                            ShortText = team.ShortName
                                                            Icon = Some <| Icons.team ""
                                                            Url = tableUrl team selectedYear 
                                                        }
                                                ))                           
                                    isSelected            
                           
                                navListMobile
                                    ({ 
                                        Header = "Sesonger"
                                        Items = years |> List.map (fun year  -> { Text = string year; Url = tableUrl selectedTeam year}                                                                   )  
                                        Footer = None                                                               
                                        IsSelected = isSelected                                                               
                                   })
                                hr []
                                h2 [] [Icons.trophy "";whitespace;encodedText t.Title]
                                br []
                                table [Striped; TableProperty.Attribute <| _class "table-table"] 
                                            [
                                                col [NoSort;Align Center;Attr <| _class "hidden-xxs"] []
                                                col [NoSort] [encodedText "Lag"]
                                                col [NoSort; Align Center] [encodedText "Kamper"]
                                                col [NoSort; Align Center; Attr <| _class "hidden-sm hidden-xs"] [encodedText "Seier"]
                                                col [NoSort; Align Center; Attr <| _class "hidden-sm hidden-xs"] [encodedText "Uavgjort"]
                                                col [NoSort; Align Center; Attr <| _class "hidden-sm hidden-xs"] [encodedText "Tap"]
                                                col [NoSort; Align Center; Attr <| _class "hidden-xs"] [encodedText "Målforskjell"]
                                                col [NoSort; Align Center] [encodedText club.Name]
                                            ]
                                            (t.Rows |> List.map (fun r ->
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
                                         [encodedText <| "Sist oppdatert: " + t.UpdatedDate.ToString("d MMM kl HH:mm") ],emptyText))
                               
                            ]
                        ]
                        sidebar [] [
                            Users.whenInRole user [Role.Admin; Role.Trener; Role.Skribent] 
                                        (fun user -> 
                                            block [] [
                                                navList ({
                                                            Header = "Admin"
                                                            Items = [
                                                                        {
                                                                            Text = [icon (fa "plus") "";whitespace;encodedText "Legg til sesong"]
                                                                            Url = "/sesong/opprett"
                                                                        }
                                                                        {
                                                                            Text = [icon (fa "edit") "";whitespace;encodedText "Rediger sesong"]
                                                                            Url = sprintf "/sesong/opprett/%i/%O" selectedYear selectedTeam.Id
                                                                        }
                                                                    ]
                                                            Footer = None
                                                            IsSelected = never
                                                        })
                                                ]) |> renderOption
                            (years.Length > 0 =?
                                (
                                block [] [
                                    navList ({ 
                                                Header = "Sesonger"
                                                Items = years |> List.map (fun year  -> { Text = [encodedText <| string year]; Url = tableUrl selectedTeam year }                                                                   )  
                                                Footer = None                                                               
                                                IsSelected = isSelected                                                               
                                           })
                                ]
                                                             
                                , emptyText))        
                            ]
                    ] 
                    |> layout club user (fun o -> { o with Title = "Tabell" }) ctx
                    |> Ok

                | None -> Error NotFound        
            | _ -> Error NotFound            