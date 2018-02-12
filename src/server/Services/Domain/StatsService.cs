using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MyTeam.Models;
using MyTeam.Models.Enums;
using MyTeam.ViewModels.Attendance;
using MyTeam.ViewModels.Stats;

namespace MyTeam.Services.Domain
{
    class StatsService : IStatsService
    {
        private readonly ApplicationDbContext _dbContext;

        public StatsService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<EventAttendanceViewModel> GetAttendance(Guid clubId, int? year = null)
        {
            var now = DateTime.Now;
            var query = _dbContext.Events
                .Where(e => e.ClubId == clubId &&
                            e.DateTime < now.AddHours(1) &&
                            e.Voluntary == false &&
                            (e.Type == EventType.Trening || e.Type == EventType.Kamp));

            if (year != null) query = query.Where(e => e.DateTime.Year == year);


            var eventIds = query.Select(e => e.Id).ToList();

            var attendances = _dbContext.EventAttendances.Where(a => eventIds.Contains(a.EventId));

            return attendances.Select(e => new EventAttendanceViewModel
            {
                DidAttend = e.DidAttend,
                EventType = e.Event.Type,
                IsAttending = e.IsAttending,
                MemberId = e.MemberId,
                WonTraining = e.WonTraining,
                IsSelected = e.IsSelected,
                Member = new AttendanceMemberViewModel
                {
                    Image = e.Member.ImageFull,
                    Id = e.MemberId,
                    Name = e.Member.Name,
                    FacebookId = e.Member.FacebookId,
                    UrlName = e.Member.UrlName,
                    Status = e.Member.Status
                }
            })
            .ToList()
            .Where(p => p.Member.Status != PlayerStatus.Trener);
        }

        public IEnumerable<int> GetAttendanceYears(Guid clubId)
        {
            return _dbContext.EventAttendances
             .Where(e => e.Event.ClubId == clubId
                    && (e.Event.Type == EventType.Trening || e.Event.Type == EventType.Kamp))
             .Select(ea => ea.Event.DateTime.Year)
             .ToList()
             .Distinct()
             .OrderByDescending(y => y);
        }

        public IEnumerable<PlayerStats> GetStats(IEnumerable<Guid> teamIds, int? selectedYear = null)
        {
            var query = _dbContext.Games.Where(g => teamIds.Contains(g.TeamId) && g.GameTypeValue != GameType.Treningskamp);

            if (selectedYear != null)
            {
                query = query.Where(g => g.DateTime.Year == selectedYear);
            }

            var games = query.Select(g => g.Id).ToList();

            var gameEvents = _dbContext.GameEvents.Where(ge => games.Contains(ge.GameId))
                                                    .ToList();

            var attendances = _dbContext.EventAttendances.Where(ea => games.Contains(ea.EventId) && ea.IsSelected)
                                                        .Select(ea => ea.MemberId)
                                                        .ToList();

            var playerIds = attendances.Select(p => p)
                                       .Distinct();

            var players = _dbContext.Members.Where(p => playerIds.Contains(p.Id))
                .Select(p => new PlayerStats
                (
                    p.Id,
                    p.FacebookId,
                    p.FirstName,
                    p.MiddleName,
                    p.LastName,
                    p.ImageFull,
                    p.UrlName,
                    gameEvents,
                    attendances
                ));

            return players.ToList();
        }


        public IEnumerable<int> GetStatsYears(IEnumerable<Guid> teamIds)
        {
            return _dbContext.GameEvents
                 .Where(e => teamIds.Contains(e.Game.TeamId) && e.Game.GameTypeValue != GameType.Treningskamp)
                 .Select(ea => ea.Game.DateTime.Year)
                 .ToList()
                 .Distinct()
                 .OrderByDescending(y => y);
        }
    }
}