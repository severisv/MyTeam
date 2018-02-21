using System;
using Microsoft.AspNetCore.Mvc;
using MyTeam.Filters;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Attendance;

namespace MyTeam.Controllers
{
    [Route("intern")]
    public class AttendanceController : BaseController
    {

        private readonly IStatsService _statsService;

        public AttendanceController(IStatsService statsService)
        {
            _statsService = statsService;
        }


        [RequireMember]
        [Route("oppmote2")]
        public IActionResult Attendance(int? year)
        {
            int selectedYear = year ?? DateTime.Now.Year;
            var eventAttendance =
                year == 0 ?
                _statsService.GetAttendance(Club.Id) :
                _statsService.GetAttendance(Club.Id, selectedYear);
            var years = _statsService.GetAttendanceYears(Club.Id);

            var model = new AttendanceViewModel(eventAttendance, years, selectedYear);

            return View("Attendance", model);
        }


    }
}