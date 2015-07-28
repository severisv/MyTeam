using System.Linq;
using Microsoft.AspNet.Mvc;
using MyTeam.Models.Domain;
using MyTeam.Services.Repositories;

namespace MyTeam.Services.Application
{
    class MemoryStore : IMemoryStore
    {
        //[FromServices]
        public IDatabaseContext DatabaseContext { get; set; }
        [FromServices]
        public IRepository<Player> PlayerRepository { get; set; }

        public MemoryStore(IRepository<Player> playerRepository)
        {
            PlayerRepository = playerRepository;
        }

        public Player GetPlayerFromUser(string name)
        {
            return PlayerRepository.Get().FirstOrDefault(p => p.UserName == name);
        }
    }
}