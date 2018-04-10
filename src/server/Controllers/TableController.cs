using System;
using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
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

        private readonly ISeasonService _seasonService;
        private readonly ITableService _tableService;
        private readonly ApplicationDbContext _dbContext;

        public TableController(ISeasonService seasonService, ITableService tableService, ApplicationDbContext dbContext)
        {
            _seasonService = seasonService;
            _tableService = tableService;
            _dbContext = dbContext;
        }


        [Route("oppdater")]
        [RequireMember(Roles.Coach, Roles.Admin)]
        public IActionResult Update(Guid seasonId)
        {
            var model = _dbContext.Seasons.Where(s => s.Id == seasonId).Select(s =>
            new CreateTableViewModel
            {
                SeasonId = seasonId,
                Name = s.Name,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                Team = s.Team.Name,
                SourceUrl = s.TableSourceUrl,
                AutoUpdate = s.AutoUpdateTable
            }).Single();
            return View(model);
        }

        [HttpPost]
        [Route("oppdatersesong")]
        [RequireMember(Roles.Coach, Roles.Admin)]
        public IActionResult UpdateSeason(UpdateSeasonViewModel model)
        {
            if (ModelState.IsValid)
            {
                _seasonService.Update(model.SeasonId, model.Name, model.AutoUpdate, model.SourceUrl);
                Alert(AlertType.Success, "Instillinger lagret");
            }
            return View("Update", model);
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
                _tableService.Update(model.SeasonId, model.ConvertTableString());
                Alert(AlertType.Success, $"{Res.Table} {Res.Saved.ToLower()}");
                return RedirectToAction("Index", "Table" , new { seasonId= model.SeasonId});
            }
            return new ErrorResult(HttpContext);
        }

    }
}  