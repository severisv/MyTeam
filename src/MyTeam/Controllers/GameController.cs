using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MyTeam.Filters;
using MyTeam.Models;
using MyTeam.Models.Enums;
using MyTeam.Models.Structs;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Game;
using MyTeam.ViewModels.Table;
using NotFoundResult = MyTeam.Extensions.Mvc.NotFoundResult;

namespace MyTeam.Controllers
{
    [Route("kamper")]
    public class GameController : BaseController
    {
     

        private readonly IGameService _gameService;
        private readonly IPlayerService _playerService;
        private readonly ApplicationDbContext _dbContext;

        public GameController(IPlayerService playerService, ApplicationDbContext dbContext,
            IGameService gameService)
        {
            _playerService = playerService;
            _dbContext = dbContext;
            _gameService = gameService;
        }



        [Route("{lag?}/{aar:int?}")]
        public IActionResult Index(string lag = null, int ? aar = null)
        {
            var team = lag ?? Club.Teams.First().ShortName;
            var teamId = Club.Teams.First(t => t.ShortName == team).Id;

            var seasons = _gameService.GetSeasons(teamId);

            var teams = Club.Teams
                .Select(t => new TeamViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    ShortName = t.ShortName
                })
                .ToList();

            var year = aar ?? seasons?.FirstOrDefault()?.Year ??  DateTime.Now.Year;
            
            var games = _gameService.GetGames(teamId, year, teams.Single(t => t.Id == teamId).Name);
            var model = new GamesViewModel(seasons, teams, year, games, team, teamId);

            return View("Index", model);
        }


        [RequireMember(Roles.Coach, Roles.Admin)]
        [Route("laguttak")]
        public IActionResult RegisterSquad(Guid eventId)
        {
            var ev = _gameService.GetRegisterSquadEventViewModel(eventId);

            var players = _playerService.GetDto(Club.Id);

            if (ev == null) return new NotFoundResult(HttpContext);

            var model = new RegisterSquadViewModel(ev, players);

            ViewBag.Title = "Laguttak";

            return View("RegisterSquad", model);
        }

        [HttpPost]
        [RequireMember(Roles.Coach, Roles.Admin, Roles.NewsWriter)]
        [Route("laguttak/velg")]
        public JsonResult SelectPlayer(Guid eventId, Guid playerId, bool isSelected)
        {
            if (eventId == Guid.Empty || playerId == Guid.Empty) return new JsonResult(JsonResponse.ValidationFailed("EventId eller PlayerId er null"));

            _gameService.SelectPlayer(eventId, playerId, isSelected);

            return new JsonResult(JsonResponse.Success());
        }

        [HttpPost]
        [RequireMember(Roles.Coach, Roles.Admin)]
        [Route("laguttak/publiser")]
        public JsonResult PublishSquad(Guid eventId)
        {
            if (eventId == Guid.Empty) return new JsonResult(JsonResponse.ValidationFailed("EventId er null"));

            _gameService.PublishSquad(eventId);

            return new JsonResult(JsonResponse.Success());
        }


        [Route("vis")]
        public IActionResult Show(Guid gameId)
        {

            var game = _gameService.GetGame(gameId);
            var model = new ShowGameViewModel(game);

            return View("Show", model);
        }

        [Route("registrerresultat")]
        public IActionResult RegisterResult(Guid gameId)
        {

            var game = _gameService.GetGame(gameId);
            var model = new ShowGameViewModel(game);

            return View("RegisterResult", model);
        }


        [Route("terminliste")]
        [RequireMember(Roles.Coach, Roles.Admin)]
        public IActionResult AddGames()
        {
            var teams = _dbContext.Teams.Where(t => t.ClubId == Club.Id).Select(t => new TeamViewModel
            {
                Id = t.Id,
                Name = t.Name,
                ShortName = t.ShortName
            }).ToList();

           var model =  new AddGamesViewModel
            {
               Teams  = teams,
               TeamId = Guid.Empty
           };
            return View(model);
        }

        [Route("terminliste")]
        [HttpPost]
        [RequireMember(Roles.Coach, Roles.Admin)]
        public IActionResult AddGames(AddGamesViewModel model)
        {
            if (ModelState.IsValid)
            {
                return View("AddGamesConfirm",  model);
            }
            return View(model);
        }

        [Route("terminliste/bekreft")]
        [HttpPost]
        [RequireMember(Roles.Coach, Roles.Admin)]
        public IActionResult AddGamesConfirm(AddGamesViewModel model)
        {
            if (ModelState.IsValid)
            {
                _gameService.AddGames(model.Games.Games, Club.Id);
                return RedirectToAction("Index", "Game", new {year = model.Games?.Games?.FirstOrDefault()?.DateTime.Year, lag = model.ShortTeamName });
            }
            return View(model);
        }
    }
}