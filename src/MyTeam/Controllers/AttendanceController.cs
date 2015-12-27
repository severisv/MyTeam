using System;
using Microsoft.AspNet.Mvc;
using MyTeam.Filters;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Attendance;

namespace MyTeam.Controllers
{
    [Route("intern")]
    public class AttendanceController : BaseController
    {
        [FromServices]
        public IStatsService StatsService { get; set; }

      
        [RequireMember]
        [Route("oppmote")]
        public IActionResult Attendance(int? year)
        {
            int selectedYear = year ?? DateTime.Now.Year;
            var eventAttendance = StatsService.GetAttendance(Club.Id, selectedYear);
            var years = StatsService.GetAttendanceYears(Club.Id);

            var model = new AttendanceViewModel(eventAttendance, years, selectedYear);
         
            return View("Attendance", model);
        }
        

    }
}