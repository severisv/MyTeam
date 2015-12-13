using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Caching.Memory;
using MyTeam.Models;
using MyTeam.Models.Domain;
using MyTeam.Models.Dto;
using MyTeam.Services.Repositories;

namespace MyTeam.Services.Application
{
    public class CacheHelper : ICacheHelper
    {
        private readonly MemoryCacheEntryOptions _cacheOptions = 
            new MemoryCacheEntryOptions {SlidingExpiration = new TimeSpan(0, 0, 0, 15) };

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
            
            member = PlayerRepository.Get().Where(p => clubId == p.Club.ClubIdentifier && p.UserName == name).Select(p => new PlayerDto(p.Id, p.Roles, p.MemberTeams.Select(mt => mt.TeamId).ToArray())).FirstOrDefault();

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

            club = ClubRepository.Get().Where(c => c.ClubIdentifier == clubId).Select(
                c => new ClubDto(c.Id, c.ClubIdentifier, c.Name, c.ShortName, c.Logo, c.Favicon, c.Teams.OrderBy(t => t.SortOrder).Select(t => t.Id))
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

        public void ClearCache(string clubId, string email)
        {
            if (string.IsNullOrEmpty(clubId) || string.IsNullOrEmpty(email)) return;
            var key = email+clubId;
            Cache.Remove(key);
        }
    }
}