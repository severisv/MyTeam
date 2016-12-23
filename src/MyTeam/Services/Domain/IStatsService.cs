using System;
using System.Collections.Generic;
using MyTeam.ViewModels.Attendance;
using MyTeam.ViewModels.Stats;

namespace MyTeam.Services.Domain
{
    public interface IStatsService
    {
        IEnumerable<EventAttendanceViewModel> GetAttendance(Guid clubId, int year);
        IEnumerable<int> GetAttendanceYears(Guid clubId);


        IEnumerable<PlayerStats> GetStats(Guid teamId, int? selectedYear = null);
        IEnumerable<int> GetStatsYears(Guid teamId);
    }
}