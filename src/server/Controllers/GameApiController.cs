using System;
using Microsoft.AspNetCore.Mvc;
using MyTeam.Filters;
using MyTeam.Models.Enums;
using MyTeam.Models.Structs;
using MyTeam.Services.Domain;

namespace MyTeam.Controllers
{
    public class GameApiController : BaseController
    {
        private readonly IGameService _gameService;

        public GameApiController(IGameService gameService)
        {
            _gameService = gameService;
        }

        public IActionResult GetSquad(Guid gameId)
        {
            var squad = _gameService.GetSquad(gameId);
            return new JsonResult(squad);

        }


        [HttpPost]
        [RequireMember(Roles.Coach)]
        public IActionResult SaveGamePlan(Guid gameId, string gamePlan)
        {
            if (ModelState.IsValid)
            {
                _gameService.SaveGamePlan(gameId, gamePlan);
                return new JsonResult(JsonResponse.Success());
            }
            return new JsonResult(JsonResponse.Failure);
        }

        [HttpPost]
        [RequireMember(Roles.Coach)]
        public IActionResult PublishGamePlan(Guid gameId)
        {
            if (ModelState.IsValid)
            {
                _gameService.PublishGamePlan(gameId);
                return new JsonResult(JsonResponse.Success());
            }
            return new JsonResult(JsonResponse.Failure);
        }

        public IActionResult GetGamePlan(Guid gameId, string gamePlan)
            => Json(_gameService.GetGamePlan(gameId));



    }
}