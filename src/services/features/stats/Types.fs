namespace MyTeam.Stats

open MyTeam
open MyTeam.Domain
open System

type SelectedYear = 
    | AllYears
    | Year of int

type SelectedTeam =
    | All of Team list
    | Team of Team


type PlayerStats = {
        Id: Guid
        FacebookId: string
        FirstName: string
        MiddleName: string
        LastName: string
        UrlName: string
        Games: int
        Goals: int
        Assists: int
        YellowCards: int
        RedCards: int
        Image: string
    }   
    with member p.Name = sprintf "%s %s" p.FirstName p.LastName    


type GetStats = Database -> SelectedTeam -> SelectedYear -> PlayerStats list

