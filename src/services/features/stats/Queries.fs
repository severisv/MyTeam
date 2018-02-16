namespace MyTeam.Stats

open MyTeam
open MyTeam.Domain
open System.Linq
open MyTeam.Models.Enums
open System

module StatsQueries =

    let get : GetStats =
        fun db selectedTeam selectedYear ->
                                 

            let teamIds = 
                let teams = match selectedTeam with
                            | Team t -> [t]
                            | All teams -> teams

                teams |> Seq.map (fun t -> t.Id)                  
                                
            
            let treningskamp = Nullable <| int GameType.Treningskamp

            let gameIds =
                match selectedYear with
                | AllYears _ ->                 
                        query {
                            for game in db.Games do
                            where (teamIds.Contains(game.TeamId) 
                                    && game.GameType <> treningskamp)
                            select(game.Id)    
                            distinct                 
                        }
                | Year year ->     
                     query {
                            for game in db.Games do
                            where (teamIds.Contains(game.TeamId) 
                                    && game.GameType <> treningskamp
                                    && year = game.DateTime.Year)
                            select(game.Id)    
                            distinct                 
                        }
                |> Seq.toList         
               
            let attendances =
                query {
                    for a in db.EventAttendances do
                    where (gameIds.Contains(a.EventId) && a.IsSelected)
                    select(a.MemberId)    
                }        
                |> Seq.toList

            let gameEvents = 
                query { 
                    for ge in db.GameEvents do
                    where (gameIds.Contains(ge.GameId))
                    select (ge)
                }                    
                |> Seq.toList

            let result = query {
                                for p in db.Players do
                                where (attendances.Contains(p.Id))
                                select (p.Id, p.FacebookId, p.FirstName, p.MiddleName, p.LastName, p.ImageFull, p.UrlName)
                            } |> Seq.toList
                              |> List.map (fun (id, facebookId, firstName, middleName, lastName, imageFull, urlName) ->
                                        {
                                            Id = id
                                            FacebookId = facebookId
                                            FirstName = firstName
                                            MiddleName = middleName
                                            LastName = lastName
                                            UrlName = urlName                                  
                                            Games = attendances |> List.filter (fun a -> a = id) |> Seq.length
                                            Goals = gameEvents |> List.filter(fun ge -> ge.Type = GameEventType.Goal && ge.PlayerId = Nullable id) |> List.length
                                            Assists = gameEvents |> List.filter (fun ge -> ge.Type = GameEventType.Goal && ge.AssistedById = Nullable id) |> List.length
                                            YellowCards = gameEvents |> List.filter (fun ge -> ge.Type = GameEventType.YellowCard && ge.PlayerId = Nullable id) |> List.length
                                            RedCards = gameEvents |> List.filter (fun ge -> ge.Type = GameEventType.RedCard && ge.PlayerId = Nullable id) |> List.length
                                            Image = imageFull
                                        }
                               )
                       
            query {
                 for p in result do
                 sortByDescending p.Games
                 thenByDescending (p.Goals + p.Assists)
                 thenByDescending p.YellowCards
                 thenByDescending p.RedCards
             } |> Seq.toList    

    