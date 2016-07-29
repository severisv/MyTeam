﻿using System;
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

        public IEnumerable<EventAttendanceViewModel> GetAttendance(Guid clubId, int year)
        {
            var now = DateTime.Now;
            var eventIds = _dbContext.Events
                .Where(e => e.ClubId == clubId &&
                            e.DateTime < now.AddHours(1) &&
                            e.DateTime.Year == year &&
                            e.Voluntary == false &&
                            (e.Type == EventType.Trening || e.Type == EventType.Kamp)).Select(e => e.Id).ToList();

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
                    UrlName = e.Member.UrlName
                }
            }).ToList();
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

        public IEnumerable<PlayerStats> GetStats(Guid teamId, int selectedYear)
        {
            var games = _dbContext.Games.Where(g => g.TeamId == teamId && g.DateTime.Year == selectedYear && g.GameType != GameType.Treningskamp)
                .Select(g => g.Id).ToList();

            var gameEvents = _dbContext.GameEvents.Where(ge => games.Contains(ge.GameId)).ToList();
            var attendances = _dbContext.EventAttendances.Where(ea => games.Contains(ea.EventId) && ea.IsSelected).Select(ea => ea.MemberId).ToList();
            
            var playerIds = attendances.Select(p => p).Distinct();

            var players = _dbContext.Members.Where(p => playerIds.Contains(p.Id))
                .Select(p => new PlayerStats
                {
                    Id = p.Id,
                    FacebookId = p.FacebookId,
                    FirstName = p.FirstName,
                    MiddleName = p.MiddleName,
                    LastName = p.LastName,
                    Image = p.ImageFull,
                    Goals = gameEvents.Count(ge => ge.Type == GameEventType.Goal && ge.PlayerId == p.Id),
                    Assists = gameEvents.Count(ge => ge.Type == GameEventType.Goal && ge.AssistedById == p.Id),
                    YellowCards = gameEvents.Count(ge => ge.Type == GameEventType.YellowCard && ge.PlayerId == p.Id),
                    RedCards = gameEvents.Count(ge => ge.Type == GameEventType.RedCard && ge.PlayerId == p.Id),
                    Games = attendances.Count(a => a == p.Id),
                    UrlName = p.UrlName
                });

            return players.ToList();
        }


        public IEnumerable<int> GetStatsYears(Guid teamId)
        {
            return _dbContext.GameEvents
                 .Where(e => e.Game.TeamId == teamId && e.Game.GameType != GameType.Treningskamp)
                 .Select(ea => ea.Game.DateTime.Year)
                 .ToList()
                 .Distinct()
                 .OrderByDescending(y => y);
        }
    }
}