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
            var teamName = lag ?? Club.Teams.First().ShortName;
            var teamId = Club.Teams.First(t => t.ShortName == teamName).Id;
            var years = _statsService.GetStatsYears(teamId).ToList();


            var selectedYear = aar ?? years.FirstOrDefault();

            var stats = 
                aar == 0 ?
                _statsService.GetStats(teamId):
                _statsService.GetStats(teamId, selectedYear);


            var model = new StatsViewModel(Club.Teams, teamName, selectedYear, years, stats);

            return View(model);
        }
        

    }
}