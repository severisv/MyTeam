namespace MyTeam.Stats

open MyTeam
open MyTeam.Domain

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

