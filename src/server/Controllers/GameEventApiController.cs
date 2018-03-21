using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MyTeam.Extensions;
using MyTeam.Filters;
using MyTeam.Models.Enums;
using MyTeam.Models.Structs;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Game;

namespace MyTeam.Controllers
{
    public class GameEventApiController : BaseController
    {

        private readonly IGameEventService _gameEventService;
       
        public GameEventApiController(IGameEventService gameEventService)
        {
            _gameEventService = gameEventService;
        }

        [HttpPost]
        [RequireMember(Roles.Admin, Roles.Coach, Roles.NewsWriter)]
        public IActionResult Add(GameEventViewModel model)
        {
            if (ModelState.IsValid)
            {
                var gameEvent = _gameEventService.AddGameEvent(model);
                return new JsonResult(gameEvent);
            }
            return new JsonResult(JsonResponse.Failure);
        }

        [HttpPost]
        [RequireMember(Roles.Admin, Roles.Coach, Roles.NewsWriter)]
        public IActionResult Delete(Guid eventId)
        {
            if (ModelState.IsValid)
            {
                _gameEventService.Delete(eventId);
                return new JsonResult(JsonResponse.Success());
            }
            return new JsonResult(JsonResponse.Failure);
        }
    }
}