using System;
using System.Linq;
using Microsoft.AspNet.Mvc;
using MyTeam.Filters;
using MyTeam.Models;
using MyTeam.Models.Enums;
using MyTeam.Resources;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Table;

namespace MyTeam.Controllers
{
    [Route("tabell")]
    public class TableController : BaseController
    {
        [FromServices]
        public ISeasonService SeasonService { get; set; }
        [FromServices]
        public ITableService TableService { get; set; }
        [FromServices]
        public ApplicationDbContext DbContext { get; set; }

        public IActionResult Index(Guid? teamId = null, Guid? seasonId = null)
        {
            var seasons = seasonId != null
                ? SeasonService.GetTeamSeasonsFromSeasonId((Guid)seasonId):
                SeasonService.GetForTeam(teamId ?? Club.TeamIds.First());

            var teams = DbContext.Teams.Where(c => c.ClubId == Club.Id).Select(t => new TeamViewModel
            {
                Id = t.Id,
                Name = t.Name
            }).ToList();

            var model = new TableViewModel(seasons, teams, seasonId);
            if (model.SelectedSeason != null)
            {
                var table = TableService.GetTable(model.SelectedSeason.Id);
                model.Table = table;
            }
            return View("Index", model);
        }


        [Route("oppdater")]
        [RequireMember(Roles.Coach, Roles.Admin)]
        public IActionResult Update(Guid seasonId)
        {
            var model = DbContext.Seasons.Where(s => s.Id == seasonId).Select(s =>
            new CreateTableViewModel
            {
                SeasonId = seasonId,
                Season = s.Name,
                Team = s.Team.Name
            }).Single();
            return View(model);
        }

        [HttpPost]
        [Route("oppdater")]
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
        [Route("lagre")]
        [RequireMember(Roles.Coach, Roles.Admin)]
        public IActionResult Save(CreateTableViewModel model)
        {
            if (ModelState.IsValid)
            {
                TableService.Create(model.SeasonId, model.TableString);

                Alert(AlertType.Success, $"{Res.Table} {Res.Saved.ToLower()}");
                return RedirectToAction("index", "Table" , new { seasonId= model.SeasonId});
            }
            return new ErrorResult(HttpContext);
        }


        [RequireMember(Roles.Coach, Roles.Admin)]
        [Route("sesong/opprett")]
        public IActionResult CreateSeason()
        {
            var model = new CreateSeasonViewModel
            {
                Teams = DbContext.Teams.Where(t => t.ClubId == Club.Id).Select(t => new TeamViewModel
                {
                    Id = t.Id,
                    Name = t.Name
                }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [Route("sesong/opprett")]
        [RequireMember(Roles.Coach, Roles.Admin)]
        public IActionResult CreateSeason(CreateSeasonViewModel model)
        {
            if (ModelState.IsValid)
            {
                TableService.CreateSeason(model.TeamId, model.Year.Value, model.Name);

                Alert(AlertType.Success, $"{Res.Season} {Res.Saved.ToLower()}");
                return Index();
            }
            return View(model);
        }


    }
}  