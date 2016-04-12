using System;
using System.Collections.Generic;
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
            new MemoryCacheEntryOptions {SlidingExpiration = new TimeSpan(0, 0, 0, 15) };

        public IMemoryCache Cache { get; set; }
        private readonly ApplicationDbContext _dbContext;

        public CacheHelper(IMemoryCache cache, ApplicationDbContext dbContext)
        {
            Cache = cache;
            _dbContext = dbContext;
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
            
            member = _dbContext.Players
                .Where(p => clubId == p.Club.ClubIdentifier && p.UserName == name)
                .Select(p => new PlayerDto(p.Id, p.FirstName,  p.ImageFull, p.FacebookId, p.Roles, p.MemberTeams.Select(mt => mt.TeamId).ToArray(), p.ProfileIsConfirmed)).FirstOrDefault();

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


            club = _dbContext.Clubs.Where(c => c.ClubIdentifier == clubId).Select(
                c => new ClubDto(c.Id, c.ClubIdentifier, c.Name, c.ShortName, c.Logo, c.Favicon, c.Teams.OrderBy(t => t.SortOrder).Select(t => new TeamDto(t.Id, t.ShortName, t.Name)).ToList())
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
            Thread.Sleep(20);
            if (string.IsNullOrEmpty(clubId) || string.IsNullOrEmpty(email)) return;
            var key = email+clubId;
            Cache.Remove(key);
        }

        public MemberNotification GetNotifications(Guid memberId, Guid clubId, IEnumerable<Guid> teamIds)
        {
            var key = clubId.ToString();

            object cachedValue;
            Cache.TryGetValue(key, out cachedValue);

            var notifications = cachedValue as Dictionary<Guid, MemberNotification>;
            if (notifications != null)
            {
                var result = notifications.TryGet(memberId);
                if (result != null) return result;
            }
            else
            {
                notifications = new Dictionary<Guid, MemberNotification>();
            }
            
            var events = new List<Guid>();
            foreach (var id in teamIds)
            {
                var ids = _dbContext.EventTeams.Where(et => et.TeamId == id)
                    .Select(et => et.Event).Where(e => (e.DateTime - DateTime.Now < new TimeSpan(4, 0, 0, 0, 0)) && (e.DateTime > DateTime.Now))
                    .Select(e => e.Id).ToList();
                events.AddRange(ids);
            }

            var currentEvents = events.Distinct().ToList();

            var count = currentEvents.Count();
            var answered = _dbContext.EventAttendances.Count(a => a.MemberId == memberId && currentEvents.Any(ce => ce == a.EventId));

            var memberNotification = new MemberNotification
            {
                UnansweredEvents = count - answered
            };

            notifications[memberId] = memberNotification;
            
            Cache.Set(key, notifications, _cacheOptions);
           
            return memberNotification;
        }

        public void ClearNotificationCache(Guid clubId)
        {
            Cache.Set(clubId.ToString(), null);
        }

        public void ClearNotificationCacheByMemberId(Guid clubId, Guid memberId)
        {
            var key = clubId.ToString();

            object cachedValue;
            Cache.TryGetValue(key, out cachedValue);

            var notifications = cachedValue as Dictionary<Guid, MemberNotification>;
            if (notifications != null)
            {
                notifications[memberId] = null;
            }
            Cache.Set(key, notifications);
        }
    }
}