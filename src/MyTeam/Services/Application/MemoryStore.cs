using System.Linq;
using Microsoft.AspNet.Mvc;
using MyTeam.Models.Domain;
using MyTeam.Services.Repositories;

namespace MyTeam.Services.Application
{
    class MemoryStore : IMemoryStore
    {
        public IDatabaseContext DatabaseContext { get; set; }
        public IRepository<Player> PlayerRepository { get; set; }
        public IRepository<Club> ClubRepository { get; set; }

        public MemoryStore(IRepository<Player> playerRepository, IRepository<Club> clubRepository)
        {
            PlayerRepository = playerRepository;
            ClubRepository = clubRepository;
        }

        public Player GetPlayerFromUser(string name)
        {
            return PlayerRepository.Get().FirstOrDefault(p => p.UserName == name);
        }

        public Club GetClub(string clubId)
        {
            return ClubRepository.Get().SingleOrDefault(c => c.ClubId == clubId);
        }
    }
}