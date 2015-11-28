using System;
using System.Linq;
using Microsoft.AspNet.Mvc;
using MyTeam.Filters;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
using MyTeam.Resources;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Table;

namespace MyTeam.Controllers
{
    public class TableController : BaseController
    {
        [FromServices]
        public ISeasonService SeasonService { get; set; }
        [FromServices]
        public ITableService TableService { get; set; }

        public IActionResult Index(Guid? teamId = null, Guid? seasonId = null)
        {
            var seasons = seasonId != null
                ? SeasonService.GetTeamSeasonsFromSeasonId((Guid)seasonId):
                SeasonService.GetForTeam(teamId ?? Club.TeamIds.First());

            var model = new TableViewModel(seasons, seasonId);
            var table = TableService.GetTable(model.SelectedSeason.Id);
            model.Table = table;


            return View("Index", model);
        }



        [RequireMember(Roles.Coach, Roles.Admin)]
        public IActionResult Update(Guid seasonId)
        {
            var season = SeasonService.Get(seasonId);

            var model = new CreateTableViewModel
            {
                SeasonId = seasonId,
                Season = season.Name,
                Team = season.Team.Name
            };
            return View(model);
        }

        [HttpPost]
        [RequireMember(Roles.Coach, Roles.Admin)]
        public IActionResult Update(CreateTableViewModel model)
        {
            if (ModelState.IsValid)
            {
                return View("UpdateConfirm", model);
            }
            return View(model);

        }

        [HttpPost]
        [RequireMember(Roles.Coach, Roles.Admin)]
        public IActionResult Save(CreateTableViewModel model)
        {
            if (ModelState.IsValid)
            {
                TableService.Create(model.SeasonId, model.TableString);

                Alert(AlertType.Success, $"{Res.Table} {Res.Saved.ToLower()}");
                return Index(seasonId: model.SeasonId);
            }
            return new ErrorResult(HttpContext);
        }


    }
}  