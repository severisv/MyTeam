using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Stats;

namespace MyTeam.Controllers
{
    [Route("statistikk")]
    public class StatsController : BaseController
    {

        private readonly IStatsService _statsService;

        public StatsController(IStatsService statsService)
        {
            _statsService = statsService;
        }

        [Route("{lag?}/{aar:int?}")]
        public IActionResult Index(string lag = null, int? aar = null)
        {
            var selectedYear = aar ?? DateTime.Now.Year;
            var teamName = lag ?? Club.Teams.First().ShortName;
            var teamId = Club.Teams.First(t => t.ShortName == teamName).Id;
            var stats = _statsService.GetStats(teamId, selectedYear);
            var years = _statsService.GetStatsYears(teamId);

            var model = new StatsViewModel(Club.Teams, teamName, selectedYear, years, stats);

            return View(model);
        }
        

    }
}