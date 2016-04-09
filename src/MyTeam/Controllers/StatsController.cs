using System;
using System.Linq;
using Microsoft.AspNet.Mvc;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Stats;

namespace MyTeam.Controllers
{
    [Route("statistikk")]
    public class StatsController : BaseController
    {
        [FromServices]
        public IStatsService StatsService { get; set; }

        [Route("{lag?}/{aar:int?}")]
        public IActionResult Index(string lag = null, int? aar = null)
        {
            var selectedYear = aar ?? DateTime.Now.Year;
            var teamName = lag ?? Club.Teams.First().ShortName;
            var teamId = Club.Teams.First(t => t.ShortName == teamName).Id;
            var stats = StatsService.GetStats(teamId, selectedYear);
            var years = StatsService.GetStatsYears(teamId);

            var model = new StatsViewModel(Club.Teams, teamName, selectedYear, years, stats);

            return View(model);
        }
        

    }
}