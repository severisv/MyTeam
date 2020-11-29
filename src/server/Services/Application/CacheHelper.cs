using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using MyTeam.Models;
using MyTeam.Models.Dto;

namespace MyTeam.Services.Application
{
    public class CacheHelper : ICacheHelper
    {
        private readonly MemoryCacheEntryOptions _cacheOptions =
            new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5.0) };

        public IMemoryCache Cache { get; set; }
        private readonly ApplicationDbContext _dbContext;

        public CacheHelper(IMemoryCache cache, ApplicationDbContext dbContext)
        {
            Cache = cache;
            _dbContext = dbContext;
        }

        private string NotificationKey(Guid clubId) => $"old-notifications-{clubId.ToString()}";

        private string GetMemberKey(Guid? clubId, string userName) => "old-user-" + clubId?.ToString() + userName;
        private string GetClubKey(string clubIdentifier) => "old-club-" + clubIdentifier;

        public PlayerDto GetPlayerFromUser(string username, Guid clubId)
        {

            if (string.IsNullOrWhiteSpace(username)) return null;

            var key = GetMemberKey(clubId, username);

            object cachedValue;
            Cache.TryGetValue(key, out cachedValue);

            if (cachedValue as bool? == false) return null;

            var member = cachedValue as PlayerDto;
            if (member != null)
            {
                return member;
            }

            member = _dbContext.Members
                .Where(p => clubId == p.ClubId && p.UserName == username)
                .Select(p => new PlayerDto(p.Id, p.FirstName, p.UrlName, p.ImageFull, p.FacebookId, p.RolesString, p.MemberTeams.Select(mt => mt.TeamId).ToArray(), p.ProfileIsConfirmed)).FirstOrDefault();

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

            var key = GetClubKey(clubId);

            object cachedValue;
            Cache.TryGetValue(key, out cachedValue);

            if (cachedValue as bool? == false) return null;

            var club = cachedValue as ClubDto;
            if (club != null)
            {
                return club;
            }


            club = _dbContext.Clubs.Where(c => c.ClubIdentifier == clubId).Select(
                c => new ClubDto(c.Id, c.ClubIdentifier, c.Name, c.ShortName, c.Logo, c.Favicon, c.Teams.OrderBy(t => t.SortOrder)
                .Select(t => new TeamDto(t.Id, t.ShortName, t.Name)).ToList())
            ).FirstOrDefault();


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

        public void ClearCache(Guid? clubId, string username)
        {
            // Sleep litt så man ikke cacher gammel data på nytt (race condition)
            Thread.Sleep(20);
            if (clubId == null || string.IsNullOrEmpty(username)) return;
            var key = GetMemberKey(clubId, username);
            Cache.Remove(key);
        }

      
    }
}
