module MyTeam.Clubs

open MyTeam
open Shared
open Shared.Domain

type Get = HttpContext -> ClubIdentifier -> Option<Club>

let get : Get =
    fun ctx clubId -> 
        let (ClubIdentifier clubId) = clubId
        let db = ctx.Database

        let clubs =
                query {
                    for club in db.Clubs do
                    where (club.ClubIdentifier = clubId)
                    join team in db.Teams on (club.Id = team.ClubId)
                    select (club, team)
                }
                |> Seq.toList
                
        let teams = clubs 
                    |> Seq.map(fun (__, team) -> 
                                {
                                  Id = team.Id
                                  ShortName = team.ShortName
                                  Name = team.Name
                                })
                    |> Seq.toList
                    |> List.sortBy (fun t -> t.Name)                                                    

        clubs 
        |> Seq.map(fun (club, __) -> 
                    {
                        Id = ClubId club.Id
                        ClubId = club.ClubIdentifier
                        ShortName = club.ShortName
                        Name = club.Name
                        Teams = teams
                        Favicon = club.Favicon
                        Logo = club.Logo
                    }) 
        |> Seq.tryHead                                  

