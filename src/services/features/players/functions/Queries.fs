namespace Services.Players

module Queries = 

    let getPlayers : GetPlayers =
        fun clubId ->
            [{
                Id = System.Guid.NewGuid()
                FirstName = "Frank"
                LastName = "Jensen"
                MiddleName = "Per"
                UrlName = "Jens2k"
                Status = Status.Aktiv
                Roles = []

            }]