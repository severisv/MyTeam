namespace MyTeam.Stats

open System.Linq
open MyTeam.Domain
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

            let attendances =
                match selectedYear with
                | AllYears _ ->                 
                        query { 
                            for game in db.Games do
                            where (teamIds.Contains(game.TeamId) && game.GameType <> treningskamp)
                            leftOuterJoin ea in db.EventAttendances on (game.Id = ea.EventId) into result
                            for ea in result do 
                            where ea.IsSelected 
                            select ea.MemberId
                        }     
                | Year year ->  
                        query { 
                            for game in db.Games do
                            where (teamIds.Contains(game.TeamId) && game.GameType <> treningskamp && year = game.DateTime.Year)
                            leftOuterJoin ea in db.EventAttendances on (game.Id = ea.EventId) into result
                            for ea in result do
                            where ea.IsSelected 
                            select ea.MemberId
                        }
                |> Seq.toList      
               

            let gameEvents =
                match selectedYear with
                | AllYears _ ->                 
                        query { 
                            for game in db.Games do
                            where (teamIds.Contains(game.TeamId) && game.GameType <> treningskamp)
                            leftOuterJoin ge in db.GameEvents on (game.Id = ge.GameId) into result
                            for ge in result do 
                            select ge
                        }     
                | Year year ->  
                        query { 
                            for game in db.Games do
                            where (teamIds.Contains(game.TeamId) && game.GameType <> treningskamp && year = game.DateTime.Year)
                            leftOuterJoin ge in db.GameEvents on (game.Id = ge.GameId) into result
                            for ge in result do
                            select ge
                        }
                |> Seq.toList             
                |> List.filter (fun ge -> not (isNull ge))  
   

            let playerIds = attendances 
                            |> Seq.distinct


            let result = query {
                                for p in db.Players do
                                where (playerIds.Contains(p.Id))
                                select (p.Id, p.FacebookId, p.FirstName, p.LastName, p.ImageFull, p.UrlName)
                            } |> Seq.toList
                              |> List.map (fun (id, facebookId, firstName, lastName, imageFull, urlName) ->
                                        {
                                            FacebookId = facebookId
                                            FirstName = firstName
                                            LastName = lastName
                                            UrlName = urlName                                  
                                            Games = attendances |> Seq.filter (fun a -> a = id) |> Seq.length
                                            Goals = gameEvents |> Seq.filter(fun ge -> ge.Type = GameEventType.Goal && ge.PlayerId = Nullable id) |> Seq.length
                                            Assists = gameEvents |> Seq.filter (fun ge -> ge.Type = GameEventType.Goal && ge.AssistedById = Nullable id) |> Seq.length
                                            YellowCards = gameEvents |> Seq.filter (fun ge -> ge.Type = GameEventType.YellowCard && ge.PlayerId = Nullable id) |> Seq.length
                                            RedCards = gameEvents |> Seq.filter (fun ge -> ge.Type = GameEventType.RedCard && ge.PlayerId = Nullable id) |> Seq.length
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

    