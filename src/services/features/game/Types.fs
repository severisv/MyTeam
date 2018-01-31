namespace MyTeam.Games

open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Members

type GameId = Guid


type GetSquad = Database -> ClubId -> GameId -> Member list