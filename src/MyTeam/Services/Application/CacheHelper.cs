using System;
using System.Linq;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Caching.Memory;
using MyTeam.Models.Domain;
using MyTeam.Models.Dto;
using MyTeam.Services.Repositories;

namespace MyTeam.Services.Application
{
    class CacheHelper : ICacheHelper
    {
        private readonly MemoryCacheEntryOptions _cacheOptions = new MemoryCacheEntryOptions {SlidingExpiration = new TimeSpan(0, 0, 1, 0) };

        public IDatabaseContext DatabaseContext { get; set; }
        public IMemoryCache Cache { get; set; }
        public IRepository<Player> PlayerRepository { get; set; }
        public IRepository<Club> ClubRepository { get; set; }

        public CacheHelper(IRepository<Player> playerRepository, IRepository<Club> clubRepository, IMemoryCache cache)
        {
            PlayerRepository = playerRepository;
            ClubRepository = clubRepository;
            Cache = cache;
        }

        public PlayerDto GetPlayerFromUser(string name, string clubId)
        {
            PlayerDto member;
            var entryExists = Cache.TryGetValue(name, out member);

            if (member != null)
            {
                member = new PlayerDto(member.Id, member.Roles);
                return member;
            }
            else if (entryExists) return null;

            if (member == null)
            {
                member = GetMemberFromDb(name, clubId);
            }

            if (member == null)
            {
                Cache.Set(name, false, _cacheOptions);
            }
            else
            {
                Cache.Set(name, member, _cacheOptions);
            }

            return member;
        }

      
        private PlayerDto GetMemberFromDb(string name, string clubId)
        {
            var player = PlayerRepository.Get().Where(p => clubId == p.Club.ClubId && p.UserName == name).Select(p => new {p.Id, p.Roles}).FirstOrDefault();
            var member = new PlayerDto(player.Id, player.Roles);
            return member;
        }

        public ClubDto GetCurrentClub(string clubId)
        {
            var club = ClubRepository.Get().Single(c => c.ClubId == clubId);

            return new ClubDto(clubId, club.Name, club.ShortName, club.Teams.OrderBy(t => t.SortOrder).Select(t => t.Id));

        }

    }
}