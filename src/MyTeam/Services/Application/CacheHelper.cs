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
            new MemoryCacheEntryOptions {SlidingExpiration = new TimeSpan(0, 0, 15, 0) };

        public IMemoryCache Cache { get; set; }
        private readonly ApplicationDbContext _dbContext;

        public CacheHelper(IMemoryCache cache, ApplicationDbContext dbContext)
        {
            Cache = cache;
            _dbContext = dbContext;
        }

        public PlayerDto GetPlayerFromUser(string username, Guid clubId)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;

            var key = username + clubId;

            object cachedValue;
            Cache.TryGetValue(key, out cachedValue);

            if (cachedValue as bool? == false) return null;

            var member = cachedValue as PlayerDto;
            if (member != null)
            {
                return member;
            }

            member = _dbContext.Players
                .Where(p => clubId == p.ClubId && p.UserName == username)
                .Select(p => new PlayerDto(p.Id, p.FirstName, p.UrlName,  p.ImageFull, p.FacebookId, p.Roles, p.MemberTeams.Select(mt => mt.TeamId).ToArray(), p.ProfileIsConfirmed)).FirstOrDefault();

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
            var key = username+clubId;

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

            var now = DateTime.Now;
            var inFourDays = now.AddDays(4);
            var events = _dbContext.Events.Where(
                    e => e.ClubId == clubId &&
                    e.DateTime  < inFourDays &&
                    (e.DateTime > now) &&
                    !e.IsPublished
                    )
                    .Select(e => e.Id).ToList();

            var ids = _dbContext.EventTeams.Where(et => events.Contains(et.EventId) && teamIds.Contains(et.TeamId)).Select(et => et.EventId).ToList().Distinct();


            var count = ids.Count();
            var answeredEvents = _dbContext.EventAttendances.Where(a => a.MemberId == memberId && ids.Contains(a.EventId)).Select(ea => ea.EventId).ToList();
            var answeredCount = answeredEvents.Count();
            var unansweredEventIds = ids.Where(i => !answeredEvents.Contains(i)).Distinct();


            var memberNotification = new MemberNotification
            {
                UnansweredEvents = count - answeredCount,
                UnansweredEventIds = unansweredEventIds
            };

            notifications[memberId] = memberNotification;

            Cache.Set(key, notifications, _cacheOptions);

            return memberNotification;
        }

        public void ClearNotificationCache(Guid clubId)
        {
            Cache.Set<Dictionary<Guid, MemberNotification>>(clubId.ToString(), null);
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

        public void ClearMemberCache(Guid memberId)
        {
            var member = _dbContext.Members.Where(m => m.Id == memberId).Select(m => new
            {
                m.ClubId,
                Name = m.Email
            }).ToList().Single();
            var key = member.Name + member.ClubId;
            Cache.Set(key, (PlayerDto)null);
        }
    }
}
