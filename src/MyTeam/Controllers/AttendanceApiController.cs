using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MyTeam.Models;
using MyTeam.Models.Enums;

namespace MyTeam.Controllers
{
    public class AttendanceApiController : BaseController
    {
        private readonly ApplicationDbContext _dbContext;

        public AttendanceApiController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public JsonResult GetRecentAttendance(Guid teamId)
        {
            var now = DateTime.Now;
            var events =
                _dbContext.Events
                .Where(e => e.Type == EventType.Trening
                        && e.DateTime <= now
                        && e.DateTime >= now.AddDays(-56)
                        && e.Voluntary == false)
                .Select(e => new { EventId = e.Id, TeamIds = e.EventTeams.Select(et => et.TeamId).ToList()})
                .ToList();

            var eventIds = events.Where(e => e.TeamIds.Contains(teamId)).Select(e => e.EventId);

            var eventAttendences = _dbContext.EventAttendances
                .Where(ea => eventIds.Contains(ea.EventId) && ea.DidAttend)
                .Select(ea => new
                {
                    EventId = ea.EventId,
                    PlayerId = ea.MemberId,
                }).ToList();

            var eventCount = eventIds.Count();

            var players = eventAttendences.GroupBy(ea => ea.PlayerId).Select(ea => ea.First());

            var data = players.Select(p => new
            {
                PlayerId = p.PlayerId,
                Attendance = GetAttendance(eventAttendences.Count(ea => ea.PlayerId == p.PlayerId), eventCount)
            });

            return new JsonResult(new { data = data });
        }

        private int GetAttendance(int count, int eventCount)
        {
            return (count*100)/eventCount;
        }
    }
}