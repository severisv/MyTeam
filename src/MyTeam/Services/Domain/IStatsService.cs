using System.Collections.Generic;
using MyTeam.Models.Domain;

namespace MyTeam.Services.Domain
{
    public interface IStatsService
    {
        IEnumerable<EventAttendance> GetAttendance(string clubId, int year);
        IEnumerable<int> GetAttendanceYears(string clubId);
    }
}