using System;
using System.Collections.Generic;
using System.Linq;
using MyTeam.Models;
using MyTeam.Models.Enums;
using MyTeam.ViewModels.Attendance;

namespace MyTeam.Services.Domain
{
    class StatsService : IStatsService
    {
        private readonly ApplicationDbContext _dbContext;

        public StatsService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<EventAttendanceViewModel> GetAttendance(IEnumerable<Guid> teamIds, int year)
        {
            var eventIds = _dbContext.Events
                .Where(e => teamIds.Any(ti => e.EventTeams.Any(et => et.TeamId == ti)) &&
                            e.DateTime.Year == year &&
                            e.Voluntary == false &&
                            (e.Type == EventType.Trening || e.Type == EventType.Kamp)).Select(e => e.Id);

            var attendances = _dbContext.EventAttendances.Where(a => eventIds.Contains(a.EventId));

            return attendances.Select(e => new EventAttendanceViewModel
            {
                DidAttend = e.DidAttend,
                EventType = e.Event.Type,
                IsAttending = e.IsAttending,
                MemberId = e.MemberId,
                Member = new AttendanceMemberViewModel
                {
                    ImageSmall = e.Member.ImageSmall,
                    Id = e.MemberId,
                    Name = e.Member.Name
                }
            }).ToList();

        }

        public IEnumerable<int> GetAttendanceYears(IEnumerable<Guid> teamIds)
        {
            return _dbContext.Events
                .Where(e => teamIds.Any(ti => e.EventTeams.Any(et => et.TeamId == ti))
                && (e.Type == EventType.Trening || e.Type == EventType.Kamp))
                .Select(e => e.DateTime.Year).ToList().Distinct();
        }
    }
}