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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

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


        [Route("vis")]
        public IActionResult Vis(Guid gameId)
        {
            var logger = HttpContext.RequestServices.GetService<ILogger<GameController>>();
            logger.LogInformation($"Videresender fra {HttpContext.Request.Path}. Referer {Request.Headers["Referer"].ToString()}  User-Agent {Request.Headers["User-Agent"].ToString()}");
            return Redirect($"/kamper/vis/{gameId}");
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