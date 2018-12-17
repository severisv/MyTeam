using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MyTeam.Filters;
using MyTeam.Models;
using MyTeam.Models.Enums;
using MyTeam.Resources;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Table;

namespace MyTeam.Controllers
{
    [Route("sesong")]
    public class SeasonController : BaseController
    {

        private readonly ISeasonService _seasonService;
        private readonly ITableService _tableService;
        private readonly ApplicationDbContext _dbContext;

        public SeasonController(ISeasonService seasonService, ITableService tableService,  ApplicationDbContext dbContext)
        {
            _seasonService = seasonService;
            _dbContext = dbContext;
            _tableService = tableService;
        }


        [Route("oppdater/{teamShortName}/{year}")]
        [RequireMember(Roles.Coach, Roles.Admin)]
        public IActionResult UpdateSeason(int year, string teamShortName)
        {
            var teamId = Club.Teams.First(c => c.ShortName == teamShortName).Id;
            var season = _seasonService.GetForTeam(teamId).FirstOrDefault(s => s.StartDate.Year == year);
            return season != null ?
                RedirectToAction("Update", new { seasonId = season.Id }) :
                RedirectToAction("CreateSeason", new { year = year, teamId = teamId });
        }

        [Route("oppdater/{seasonId}")]
        [RequireMember(Roles.Coach, Roles.Admin)]
        public IActionResult Update(Guid seasonId)
        {
            var model = _dbContext.Seasons.Where(s => s.Id == seasonId).Select(s =>
            new UpdateSeasonViewModel
            {
                SeasonId = seasonId,
                Name = s.Name,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                Team = s.Team.Name,
                SourceUrl = s.TableSourceUrl,
                AutoUpdate = s.AutoUpdateTable,
                AutoUpdateFixtures = s.AutoUpdateFixtures,
                FixturesSourceUrl = s.FixturesSourceUrl
            }).Single();
            return View(model);
        }

        [HttpPost]
        [Route("oppdater")]
        [RequireMember(Roles.Coach, Roles.Admin)]
        public IActionResult UpdateSeason(UpdateSeasonViewModel model)
        {
            if (ModelState.IsValid)
            {
                _seasonService.Update(model.SeasonId, model.Name, model.AutoUpdate, model.SourceUrl, model.AutoUpdateFixtures, model.FixturesSourceUrl);
                // _fixtureService.RefreshFixtures();
                // _tableService.RefreshTables();

                Alert(AlertType.Success, "Instillinger lagret");
            }
            return View("Update", model);
        }


        [Route("slett")]
        [RequireMember(Roles.Coach, Roles.Admin)]
        public IActionResult Delete(Guid seasonId)
        {
            _seasonService.Delete(seasonId);
            return RedirectToAction("Index", "Table");
        }



        [RequireMember(Roles.Coach, Roles.Admin)]
        [Route("opprett/{year:int?}/{teamId:Guid?}")]
        public IActionResult CreateSeason(int? year = null, Guid? teamId = null)
        {
            var model = new CreateSeasonViewModel
            {
                Teams = _dbContext.Teams.Where(t => t.ClubId == Club.Id).Select(t => new TeamViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                }).ToList(),
                Year = null,
            };
            if (teamId != null) model.TeamId = teamId.Value;
            return View("Create", model);
        }

        [HttpPost]
        [Route("opprett")]
        [RequireMember(Roles.Coach, Roles.Admin)]
        public IActionResult Create(CreateSeasonViewModel model)
        {
            if (ModelState.IsValid)
            {
                _seasonService.CreateSeason(model.TeamId, model.Year.Value, model.Name, model.AutoUpdate, model.SourceUrl, model.AutoUpdateFixtures, model.FixturesSourceUrl);
                // _fixtureService.RefreshFixtures();
                // _tableService.RefreshTables();

                Alert(AlertType.Success, $"{Res.Season} {Res.Saved.ToLower()}");
                return RedirectToAction("Index", "Table", new { teamId = model.TeamId });
            }
            return View(model);
        }

    }


}