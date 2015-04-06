using System.Collections.Generic;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;

namespace MyTeam.Services.Repositories
{
    public static class TestData
    {
        private static Club _club =
            new Club()
            {
                ShortName = "Wam-Kam",
                Name = "Wam-Kam FK",
                Players = new List<Player>()
            };

        public static void Addto(TestRepository testRepository)
        {
            AddClub(testRepository);
            AddPlayers(testRepository);
        }

        private static void AddClub(TestRepository testRepository)
        {
        testRepository.Add(_club);
        }   

        private static void AddPlayers(TestRepository testRepository)
        {
           testRepository.Add(GetPlayer("Tom", "Hansen"));
           testRepository.Add(GetPlayer("Severin", "Sverdvik", imageName: "severin"));
           testRepository.Add(GetPlayer("Fredrik", "Hansen"));
           testRepository.Add(GetPlayer("Tom", "Lund"));
           testRepository.Add(GetPlayer("Freddy", "Dos Santos"));
           testRepository.Add(GetPlayer("Ole Jørgen", "Grumstad")); 
           testRepository.Add(GetPlayer("Ville", "Borring", "Lande"));
           testRepository.Add(GetPlayer("Snorre", "Edwin", "Lothar von Gohren"));
           testRepository.Add(GetPlayer("Andre", "Uberg"));
           testRepository.Add(GetPlayer("Sindre", "Meldalen", "Granly"));
           testRepository.Add(GetPlayer("Hans Petter", "Olsen"));
           testRepository.Add(GetPlayer("Erik", "Nakkerud"));
    
        }

        private static Player GetPlayer(string fornavn, string etternavn, string mellomnavn = "", string imageName = null)
        {
            var player = new Player()
            {
                FirstName = fornavn,
                MiddleName = mellomnavn,
                LastName = etternavn,
                Imagename = imageName,
                Club = _club,
                Positions = new List<Position>
                {
                    Position.Spiss, Position.Ving
                }
            };
            _club.Players.Add(player);
            return player;
        }
    }
}