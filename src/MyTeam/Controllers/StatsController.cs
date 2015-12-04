using System;
using System.Linq;
using Microsoft.AspNet.Mvc;
using MyTeam.Filters;
using MyTeam.Models.Enums;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Attendance;
using MyTeam.ViewModels.Table;

namespace MyTeam.Controllers
{
    [Route("")]
    public class StatsController : BaseController
    {
        [FromServices]
        public IStatsService StatsService { get; set; }

        [Route("statistikk")]
        public IActionResult Index()
        {
            Alert(AlertType.Info, "Det er ikke registrert noe statistikk enda");
            return View();
        }

        [RequireMember]
        [Route("intern/oppmote")]
        public IActionResult Attendance(int? year)
        {
            int selectedYear = year ?? DateTime.Now.Year;
            var eventAttendance = StatsService.GetAttendance(HttpContext.GetClub().Id, selectedYear);
            var years = StatsService.GetAttendanceYears(HttpContext.GetClub().Id);

            var model = new AttendanceViewModel(eventAttendance, years, selectedYear);
         
            return View("Attendance", model);
        }
        

    }
}