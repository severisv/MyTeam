using System;
using System.Collections.Generic;
using MyTeam.Models.Domain;
using MyTeam.ViewModels.Attendance;

namespace MyTeam.Services.Domain
{
    public interface IStatsService
    {
        IEnumerable<EventAttendanceViewModel> GetAttendance(Guid clubId, int year);
        IEnumerable<int> GetAttendanceYears(Guid clubId);
        

    }
}