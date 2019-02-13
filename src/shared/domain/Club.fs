namespace Shared.Domain

open System

type TeamId = Guid

type Team = {
        Id: TeamId
        ShortName: string
        Name: string
}

type ClubIdentifier = ClubIdentifier of string
type ClubId = ClubId of Guid

type Club = {
         Id: ClubId    
         ClubId: string    
         ShortName: string    
         Name: string    
         Teams: Team list
         Favicon: string    
         Logo: string    
}