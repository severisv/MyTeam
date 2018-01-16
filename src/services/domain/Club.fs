namespace MyTeam.Domain

open System

type TeamId = Guid

type Team = {
        Id: TeamId
        ShortName: string
        Name: string
}

type ClubIdentifier = string
type ClubId = Guid

type Club = {
         Id: ClubId    
         ClubId: string    
         ShortName: string    
         Name: string    
         Teams: Team list
         Favicon: string    
         Logo: string    
}