using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using MyTeam.Models;
using MyTeam.Models.Enums;
using MyTeam.Services.Domain;

namespace MyTeam.Controllers
{
    public class AttendanceApiController : BaseController
    {
        [FromServices]
        public ApplicationDbContext DbContext { get; set; }
        
        public JsonResult GetRecentAttendance(Guid teamId)
        {
            var events =
                DbContext.Events
                .Where(e => e.Type == EventType.Trening 
                        && e.DateTime <= DateTime.Now 
                        && e.DateTime >= DateTime.Now.AddDays(-42)
                        && e.Voluntary == false)
                .Select(e => new { EventId = e.Id, TeamIds = e.EventTeams.Select(et => et.TeamId).ToList()})
                .ToList();
            
            var eventIds = events.Where(e => e.TeamIds.Contains(teamId)).Select(e => e.EventId);

            var eventAttendences = DbContext.EventAttendances
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