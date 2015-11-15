using System;
using System.Linq;
using Microsoft.AspNet.Mvc;
using MyTeam.Filters;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Attendance;
using MyTeam.ViewModels.Table;

namespace MyTeam.Controllers
{
    public class StatsController : BaseController
    {
        [FromServices]
        public IStatsService StatsService { get; set; }
  
        [RequireMember]
        public IActionResult Attendance(int? year)
        {
            int selectedYear = year ?? DateTime.Now.Year;
            var eventAttendance = StatsService.GetAttendance(Context.GetClub().ClubId, selectedYear);
            var years = StatsService.GetAttendanceYears(Context.GetClub().ClubId);

            var model = new AttendanceViewModel(eventAttendance, years, selectedYear);
         
            return View("Attendance", model);
        }
        

    }
}