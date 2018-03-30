namespace MyTeam.Domain

open System

type TeamId = Guid

type Team = {
        Id: TeamId
        ShortName: string
        Name: string
}

type SelectedYear = 
    | AllYears
    | Year of int

type SelectedTeam =
    | All of Team list
    | Team of Team


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