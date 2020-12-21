module MyTeam.Clubs

open MyTeam
open Shared
open Shared.Domain

type Get = HttpContext -> ClubIdentifier -> Option<Club>

let asLeagueType = function
    | Models.Domain.Formasjon.FireTreTre -> Ellever
    | Models.Domain.Formasjon.FireFireTo -> Ellever
    | Models.Domain.Formasjon.TreToEn  -> Syver
    | Models.Domain.Formasjon.ToTreEn  -> Syver
    | _ -> Ellever

let get : Get =
    fun ctx clubId -> 
        let (ClubIdentifier clubId) = clubId
        let db = ctx.Database
        
        let teams = query { for t in db.Teams do select t } |> Seq.toList

        let clubs =
                query {
                    for club in db.Clubs do
                    where (club.ClubIdentifier = clubId)
                    join team in teams on (club.Id = team.ClubId)
                    select ({| Id = club.Id
                               ClubIdentifier = club.ClubIdentifier
                               ShortName = club.ShortName
                               Name = club.Name
                               Favicon = club.Favicon
                               Logo = club.Logo |},
                            {| Formation = team.Formation; Id = team.Id; ShortName = team.ShortName; Name = team.Name |})
                }
                |> Seq.toList
                
        let teams = clubs 
                    |> Seq.map(fun (__, team) -> 
                                {
                                  Id = team.Id
                                  ShortName = team.ShortName
                                  Name = team.Name
                                  LeagueType = team.Formation |> asLeagueType
                                })
                    |> Seq.toList
                    |> List.sortBy (fun t -> t.Name)                                                    

        clubs 
        |> Seq.map(fun (club, __) -> 
                    { Id = ClubId club.Id
                      ClubId = club.ClubIdentifier
                      ShortName = club.ShortName
                      Name = club.Name
                      Teams = teams
                      Favicon = club.Favicon
                      Logo = club.Logo }) 
        |> Seq.tryHead                                  

