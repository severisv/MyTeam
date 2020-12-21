namespace MyTeam.Stats

open System.Linq
open MyTeam.Models.Enums
open System

module StatsQueries =

    let get : GetStats =
        fun db selectedTeam selectedYear ->
                              
            let teamIds = 
               match selectedTeam with
               | Team t -> [t]
               | Seven teams -> teams
               | Elleven teams -> teams
               |> List.map (fun t -> t.Id)                  
                                
            
            let treningskamp = Nullable <| int GameType.Treningskamp

            let now = DateTime.Now

            let attendances =
                match selectedYear with
                | AllYears _ ->                 
                        query { 
                            for game in db.Games do
                            where (teamIds.Contains(game.TeamId.Value) && game.GameType <> treningskamp && game.DateTime < now)
                            join ea in db.EventAttendances on (game.Id = ea.EventId)
                            where ea.IsSelected 
                            select ea.MemberId
                        }     
                | Year year ->  
                        query { 
                            for game in db.Games do
                            where (teamIds.Contains(game.TeamId.Value) && game.GameType <> treningskamp && year = game.DateTime.Year  && game.DateTime < now)
                            join ea in db.EventAttendances on (game.Id = ea.EventId)
                            where ea.IsSelected 
                            select ea.MemberId
                        }
                |> Seq.toList      
               

            let gameEvents =
                match selectedYear with
                | AllYears _ ->                 
                        query { 
                            for game in db.Games do
                            where (teamIds.Contains(game.TeamId.Value) && game.GameType <> treningskamp)
                            join ge in db.GameEvents on (game.Id = ge.GameId)
                            select ge
                        }     
                | Year year ->  
                        query { 
                            for game in db.Games do
                            where (teamIds.Contains(game.TeamId.Value) && game.GameType <> treningskamp && year = game.DateTime.Year)
                            join ge in db.GameEvents on (game.Id = ge.GameId)
                            select ge
                        }
                |> Seq.toList             
                |> List.filter (fun ge -> not (isNull ge))  
   

            let playerIds = attendances 
                            |> List.distinct


            let result =
                query {
                    for p in db.Members do
                    where (playerIds.Contains(p.Id))
                    select (p.Id, p.FacebookId, p.FirstName, p.LastName, p.ImageFull, p.UrlName)
                } |> Seq.toList
                  |> List.map (fun (id, facebookId, firstName, lastName, imageFull, urlName) ->
                            {   FacebookId = facebookId
                                FirstName = firstName
                                LastName = lastName
                                UrlName = urlName                                  
                                Games = attendances |> List.filter (fun a -> a = id) |> List.length
                                Goals = gameEvents |> List.filter(fun ge -> ge.Type = GameEventType.Goal && ge.PlayerId = Nullable id) |> Seq.length
                                Assists = gameEvents |> List.filter (fun ge -> ge.Type = GameEventType.Goal && ge.AssistedById = Nullable id) |> Seq.length
                                YellowCards = gameEvents |> List.filter (fun ge -> ge.Type = GameEventType.YellowCard && ge.PlayerId = Nullable id) |> Seq.length
                                RedCards = gameEvents |> List.filter (fun ge -> ge.Type = GameEventType.RedCard && ge.PlayerId = Nullable id) |> Seq.length
                                Image = imageFull })
                       
            query {
                 for p in result do
                 sortByDescending (match selectedYear with | AllYears -> p.Games | _ ->  (p.Goals + p.Assists))
                 thenByDescending (match selectedYear with | Year _ -> p.Games | _ ->  (p.Goals + p.Assists))
                 thenByDescending p.YellowCards
                 thenByDescending p.RedCards
             } |> Seq.toList    

    