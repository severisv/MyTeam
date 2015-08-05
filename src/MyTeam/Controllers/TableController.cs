using System;
using System.Linq;
using Microsoft.AspNet.Mvc;
using MyTeam.Models.Domain;
using MyTeam.Services.Application;
using MyTeam.Services.Domain;
using MyTeam.Services.Repositories;
using MyTeam.ViewModels.Table;

namespace MyTeam.Controllers
{
    public class TableController : BaseController
    {
        [FromServices]
        public ISeasonService SeasonService { get; set; }

        public IActionResult Index(Guid? teamId = null, Guid? seasonId = null)
        {

            var seasons = SeasonService.Get(teamId ?? Club.TeamIds.First());
            
            var model = new TableViewModel(seasons, seasonId);
            
            return View(model);
        }

        public IActionResult Update(Guid seasonId)
        {
            return View(seasonId);
        }

        [HttpPost]
        public IActionResult Update(Guid seasonId, string table)
        {
            var model = new Table(seasonId, table);
            
            return View("UpdateConfirm", model);
        }


    }
}  