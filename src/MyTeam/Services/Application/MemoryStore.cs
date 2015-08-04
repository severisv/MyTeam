using System;
using System.Linq;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using MyTeam.Models.Domain;
using MyTeam.Models.Dto;
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

        public PlayerDto GetPlayerFromUser(string name)
        {
            var playerId = PlayerRepository.Get().Where(p => p.UserName == name).Select(p => p.Id).FirstOrDefault();

            if (playerId == Guid.Empty) return null;
            return new PlayerDto(playerId);
        }

        public ClubDto GetCurrentClub(string clubId)
        {
            var club = ClubRepository.Get().Single(c => c.ClubId == clubId);

            return new ClubDto(clubId, club.Name, club.ShortName, club.Teams.OrderBy(t => t.SortOrder).Select(t => t.Id));

        }

    }
}