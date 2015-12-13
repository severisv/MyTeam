using System;
using System.Collections.Generic;
using MyTeam.Models.Domain;
using MyTeam.ViewModels.Attendance;

namespace MyTeam.Services.Domain
{
    public interface IStatsService
    {
        IEnumerable<EventAttendanceViewModel> GetAttendance(IEnumerable<Guid> teamIds, int year);
        IEnumerable<int> GetAttendanceYears(IEnumerable<Guid> teamIds);
    }
}