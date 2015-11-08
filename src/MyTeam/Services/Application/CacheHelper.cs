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
        private readonly MemoryCacheEntryOptions _cacheOptions = 
            new MemoryCacheEntryOptions {SlidingExpiration = new TimeSpan(0, 0, 0, 15) };

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
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(clubId)) return null;

            var key = name + clubId;

            object cachedValue;
            Cache.TryGetValue(key, out cachedValue);

            if (cachedValue as bool? == false) return null;

            var member = cachedValue as PlayerDto;
            if (member != null)
            {
                return member;
            }
            
             member = PlayerRepository.Get().Where(p => clubId == p.Club.ClubId && p.UserName == name).Select(p => new PlayerDto(p.Id, p.Roles)).FirstOrDefault();

            if (member != null)
            {
                Cache.Set(key, member, _cacheOptions);
            }
            else
            {
                Cache.Set(key, false, _cacheOptions);
            }
            return member;
        }

    
        public ClubDto GetCurrentClub(string clubId)
        {
            if (string.IsNullOrWhiteSpace(clubId)) return null;

            var key = clubId;

            object cachedValue;
            Cache.TryGetValue(key, out cachedValue);

            if (cachedValue as bool? == false) return null;

            var club = cachedValue as ClubDto;
            if (club != null)
            {
                return club;
            }

            club = ClubRepository.Get().Where(c => c.ClubId == clubId).Select(
                c => new ClubDto(clubId, c.Name, c.ShortName, c.Teams.OrderBy(t => t.SortOrder).Select(t => t.Id))
            ).Single();


            if (club != null)
            {
                Cache.Set(key, club, _cacheOptions);
            }
            else
            {
                Cache.Set(key, false, _cacheOptions);
            }
            return club;

        }

    }
}