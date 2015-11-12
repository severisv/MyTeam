using System;
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
                ClubId = "wamkam",
                Players = new List<Player>(),
                Teams = new List<Team> { new Team{Name = "Wam-Kam 2",SortOrder = 2} , new Team { Name = "Wam-Kam 1", SortOrder = 1 } }
            };

        public static void Addto(TestRepository testRepository)
        {
            AddClub(testRepository);
            AddPlayers(testRepository);
            AddEvents(testRepository);
        }

        private static void AddClub(TestRepository testRepository)
        {
        testRepository.Add(_club);
            foreach (var team in _club.Teams)
            {
                testRepository.Add(CreateSeason(team));
            }
        }

        private static Season CreateSeason(Team team)
        {
            return new Season {StartDate = new DateTime(2015,01,01), EndDate = new DateTime(2015,12,31), Team = team, TeamId = team.Id};
        }

        private static void AddPlayers(TestRepository testRepository)
        {
           testRepository.Add(GetPlayer("Severin", "Sverdvik", username: "severin@sverdvik.no", roleString: Roles.Coach));
           testRepository.Add(GetPlayer("Tom", "Hansen"));
           testRepository.Add(GetPlayer("Fredrik", "Hansen"));
           testRepository.Add(GetPlayer("Tom", "Lund"));
           testRepository.Add(GetPlayer("Freddy", "Dos Santos"));
           testRepository.Add(GetPlayer("Ole Jorgen", "Grumstad")); 
           testRepository.Add(GetPlayer("Ville", "Borring", "Lande"));
           testRepository.Add(GetPlayer("Snorre", "Edwin", "Lothar von Gohren"));
           testRepository.Add(GetPlayer("Andre", "Uberg"));
           testRepository.Add(GetPlayer("Sindre", "Meldalen", "Granly"));
           testRepository.Add(GetPlayer("Hans Petter", "Olsen"));
           testRepository.Add(GetPlayer("Erik", "Nakkerud"));
           testRepository.Add(GetPlayer("Oystein", "Bondhus", status: PlayerStatus.Pensjonert));
  
    
        }


        private static Player GetPlayer(string fornavn, string etternavn, string mellomnavn = "", string imageName = null, PlayerStatus status = PlayerStatus.Aktiv, string username = "", string roleString = "")
        {
            var player = new Player()
            {
                FirstName = fornavn,
                MiddleName = mellomnavn,
                LastName = etternavn,
                Imagename = imageName,
                Status = status,
                Club = _club,
                UserName = username,
                ImageSmall = "~/img/default_player.gif",
                ImageMedium = "~/img/default_player.gif",
                ImageFull = "~/img/default_player.gif",
                Positions = new List<Position>
                {
                    Position.Spiss, Position.Ving
                }
                ,
                RolesString = roleString

        };
            _club.Players.Add(player);
            return player;
        }


        private static void AddEvents(TestRepository testRepository)
        {
            var players = testRepository.Get<Player>();


            testRepository.Add(CreateEvent(testRepository,D(-14), location: "Muselunden"));
            testRepository.Add(CreateEvent(testRepository,D(-7), location: "Muselunden"));
            testRepository.Add(CreateEvent(testRepository,D(-1), location: "Muselunden", players: players));
            testRepository.Add(CreateEvent(testRepository,DateTime.Now.AddHours(1), location: "Muselunden"));
            testRepository.Add(CreateEvent(testRepository,DateTime.Now.AddHours(-1), location: "Muselunden"));
            testRepository.Add(CreateEvent(testRepository,D(+7), location: "Muselunden"));
            testRepository.Add(CreateEvent(testRepository, D(+14), location: "Muselunden"));

        }

        private static DateTime D(int offset)
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19,30,0).AddDays(offset);
        }

        private static Event CreateEvent(TestRepository testRepository, DateTime dateTime, string description = "", string location = "", EventType type = EventType.Trening, Player[] players = null)
        {
            var result = new Training()
            {
                DateTime = dateTime,
                Description = description,
                Location = location,
                Type = type,
            };

            var attendees = new List<EventAttendance>();

            players = players ?? new Player[0];

            foreach (var player in players)
            {
                attendees.Add(new EventAttendance
                {
                    Player = player,
                    PlayerId = player.Id,
                    EventId = result.Id,
                    Event = result
                });
            }

            testRepository.Add(attendees.ToArray());

            result.Attendees = attendees;

            

            return result;
        }
    }
}