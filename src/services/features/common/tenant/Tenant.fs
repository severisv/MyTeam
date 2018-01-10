namespace MyTeam

open System
open Microsoft.AspNetCore.Http

module Tenant =

    type Team = {
        Id: Guid
        ShortName: string
        Name: string
    }

    type Club = {
             Id: Guid    
             ClubId: string    
             ShortName: string    
             Name: string    
             Teams: Team list
             Favicon: string    
             Logo: string    
    }

    type GetClub = HttpContext -> Option<Club>


    let getClubId (ctx: HttpContext) =
        // let hostNameArray = ctx.Request.Host.Value.Split('.')
        let hostNameArray = ["www";"wamkam";".no"]
        if hostNameArray.Length > 2 then
            if "www".EqualsIc(hostNameArray.[0]) then
                Some(hostNameArray.[1])
            else 
                Some(hostNameArray.[0])
        else 
            None                


    let getClub : GetClub =
        fun ctx ->
            getClubId ctx 
            |> Option.bind (fun clubId -> 
                                printf "%s" clubId
                                let clubQuery () = 
                                    let connectionString = getConnectionString ctx
                                    let database = Database.get connectionString

                                    let clubs =
                                            query {
                                                for club in database.Dbo.Club do
                                                where (club.ClubIdentifier = clubId)
                                                join team in database.Dbo.Team on (club.Id = team.ClubId)
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

                                    clubs 
                                    |> Seq.map(fun (club, __) -> 
                                        {
                                            Id = club.Id
                                            ClubId = club.ClubIdentifier
                                            ShortName = club.ShortName
                                            Name = club.Name
                                            Teams = teams
                                            Favicon = club.Name
                                            Logo = club.Logo
                                        }) 
                                    |> Seq.tryHead                                  
                                                                                                         
                                    
                                Cache.get ctx ("club-" + clubId) clubQuery
                           )
                            