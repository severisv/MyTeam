using System;
using System.Collections.Generic;
using MyTeam.Models.Domain;

namespace MyTeam.Services.Domain
{
    public interface IStatsService
    {
        IEnumerable<EventAttendance> GetAttendance(Guid clubId, int year);
        IEnumerable<int> GetAttendanceYears(Guid clubId);
    }
}