using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Stats;

namespace MyTeam.Controllers
{
    [Route("stats")]
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

            var teamIds = (teamName == "total" ?
                Club.Teams.Select(t => t.Id) :
                Club.Teams.Where(t => t.ShortName == teamName).Select(t => t.Id)
                ).ToList();

            var years = _statsService.GetStatsYears(teamIds).ToList();


            var selectedYear = aar ?? years.FirstOrDefault();

            var stats =
                aar == 0 ?
                _statsService.GetStats(teamIds) :
                _statsService.GetStats(teamIds, selectedYear);


            var model = new StatsViewModel(Club.Teams, teamName, selectedYear, years, stats);

            return View(model);
        }


    }
}