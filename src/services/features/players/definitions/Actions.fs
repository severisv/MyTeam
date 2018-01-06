namespace Services.Players

open Services
open Services.Domain

type GetPlayers = ConnectionString -> ClubId -> Player list