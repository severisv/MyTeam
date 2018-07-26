using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MyTeam.Filters;
using MyTeam.Models;
using MyTeam.Models.Dto;
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



        [Route("{lagnavn?}/{aar:int?}")]
        public IActionResult Index(string lagnavn = null, int? aar = null)
        {
            var team = lagnavn ?? Club.Teams.First().ShortName;
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

            var year = aar ?? seasons?.FirstOrDefault()?.Year ?? DateTime.Now.Year;

            var games = _gameService.GetGames(teamId, year, teams.Single(t => t.Id == teamId).Name);
            var model = new GamesViewModel(seasons, teams, year, games, team, teamId);

            return View("Index", model);
        }


        [RequireMember(Roles.Coach, Roles.Admin)]
        [Route("laguttak")]
        public IActionResult RegisterSquad(Guid eventId)
        {
            var ev = _gameService.GetRegisterSquadEventViewModel(eventId);

            var players = _playerService.GetDto(Club.Id, includeCoaches: true);

            if (ev == null) return new NotFoundResult(HttpContext);

            var model = new RegisterSquadViewModel(ev, players.ToList());

            ViewBag.Title = "Laguttak";

            return View("RegisterSquad", model);
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


        [Route("registrerresultat")]
        public IActionResult RegisterResult(Guid gameId)
        {

            var game = _gameService.GetGame(gameId);
            var model = new ShowGameViewModel(game);

            return View("RegisterResult", model);
        }



        [Route("bytteplan")]
        [RequireMember]
        public IActionResult GamePlan()
        {
            var date = DateTime.Now.AddHours(-3);
            var gameId =
                ControllerContext.HttpContext.UserIsInRole(Roles.Coach) ?
                _dbContext.Games.Where(g => g.DateTime >= date).OrderBy(g => g.DateTime).Select(g => g.Id).FirstOrDefault() :
                _dbContext.Games.OrderByDescending(g => g.DateTime).Where(g => g.GamePlanIsPublished != null).Select(g => g.Id).FirstOrDefault();

            if (gameId != null) return RedirectToAction("GamePlanForGame", new { gameId = gameId });

            return View("GamePlan");
        }

        [Route("bytteplan/{gameId}")]
        [RequireMember]
        public IActionResult GamePlanForGame(Guid gameId)
        {
            var game = _dbContext.Games.Where(g => g.Id == gameId)
                .Select(g =>
                    new
                    {
                        TeamId = g.TeamId,
                        Opponent = g.Opponent,
                        GamePlan = g.GamePlan,
                        GamePlanIsPublished = g.GamePlanIsPublished,
                        Players = g.Attendees.Where(p => p.IsSelected).Select(p =>
                            new SquadMember(
                                p.MemberId,
                                p.Member.FirstName,
                                p.Member.LastName,
                                p.Member.Image,
                                p.Member.FacebookId
                               ))
                    })
                .ToList()
                .Single();


            var model = new GamePlanViewModel(
                gameId,
                Club.Teams.Single(t => t.Id == game.TeamId).ShortName,
                game.Opponent,
                game.GamePlan,
                game.GamePlanIsPublished,
                game.Players,
                this.HttpContext.Cloudinary()
                );


            if (model == null || (!ControllerContext.HttpContext.UserIsInRole(Roles.Coach) && !model.IsPublished))
                return new NotFoundResult(HttpContext);

            return View("GamePlan", model);
        }
    }
}