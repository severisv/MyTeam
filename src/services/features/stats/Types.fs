namespace MyTeam.Stats

open MyTeam
open Shared.Domain

type SelectedYear = 
    | AllYears
    | Year of int

type SelectedTeam =
    | Seven of Team list
    | Elleven of Team list
    | Team of Team


type PlayerStats = {
        FacebookId: string
        FirstName: string
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

