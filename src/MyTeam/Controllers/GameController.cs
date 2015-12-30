using System;
using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using MyTeam.Filters;
using MyTeam.Models;
using MyTeam.Models.Enums;
using MyTeam.Models.Structs;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Game;
using MyTeam.ViewModels.Table;

namespace MyTeam.Controllers
{
    [Route("kamper")]
    public class GameController : BaseController
    {
        [FromServices]
        public IEventService EventService { get; set; }
        [FromServices]
        public IPlayerService PlayerService { get; set; }
        [FromServices]
        public IGameService GameService { get; set; }
        [FromServices]
        public ISeasonService SeasonService { get; set; }
        [FromServices]
        public ApplicationDbContext DbContext { get; set; }

        [Route("")]
        public IActionResult Index(int? year = null, Guid? teamId = null)
        {
            year = year ?? DateTime.Now.Year;
            teamId = teamId ?? Club.TeamIds.First();

            var seasons = GameService.GetSeasons(teamId.Value);

            var teams = DbContext.Teams.OrderBy(t => t.SortOrder)
                .Where(c => c.ClubId == Club.Id).Select(t => new TeamViewModel
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .ToList();

            var games = GameService.GetGames(teamId.Value, year.Value);
            var model = new GamesViewModel(seasons, teams, year.Value, games, teamId.Value);

            return View("Index", model);
        }


        [RequireMember(Roles.Coach, Roles.Admin)]
        [Route("laguttak")]
        public IActionResult RegisterSquad(Guid eventId)
        {
            var ev = GameService.GetRegisterSquadEventViewModel(eventId);

            var players = PlayerService.GetDto(Club.ClubId);

            if (ev == null) return new NotFoundResult(HttpContext);

            var model = new RegisterSquadViewModel(ev, players);

            ViewBag.Title = "Laguttak";

            return View("RegisterSquad", model);
        }

        [HttpPost]
        [RequireMember(Roles.Coach, Roles.Admin)]
        [Route("laguttak/velg")]
        public JsonResult SelectPlayer(Guid eventId, Guid playerId, bool isSelected)
        {
            if (eventId == Guid.Empty || playerId == Guid.Empty) return new JsonResult(JsonResponse.ValidationFailed("EventId eller PlayerId er null"));

            GameService.SelectPlayer(eventId, playerId, isSelected);

            return new JsonResult(JsonResponse.Success());
        }

        [HttpPost]
        [RequireMember(Roles.Coach, Roles.Admin)]
        [Route("laguttak/publiser")]
        public JsonResult PublishSquad(Guid eventId)
        {
            if (eventId == Guid.Empty) return new JsonResult(JsonResponse.ValidationFailed("EventId er null"));

            GameService.PublishSquad(eventId);

            return new JsonResult(JsonResponse.Success());
        }

    }
}