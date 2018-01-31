namespace MyTeam.Games

open MyTeam
open MyTeam.Domain

type GameId = Guid

type ScoreForm = {
    Home: int Option 
    Away: int Option
}

type SetScore = Database -> ClubId -> GameId -> ScoreForm -> Result<unit, Error>
