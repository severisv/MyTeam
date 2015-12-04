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
    [Route("kamper")]
    public class GameController : BaseController
    {
        [FromServices]
        public IStatsService StatsService { get; set; }

        [Route("")]
        public IActionResult Index()
        {
            Alert(AlertType.Info, "Det er ikke registrert noen kamper enda");
            return View();
        }
        

    }
}