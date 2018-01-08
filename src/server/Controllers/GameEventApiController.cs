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

        public IActionResult GetTypes()
        {
            var gameEventTypes = Enum.GetValues(typeof (GameEventType)).Cast<GameEventType>()
                .Select(e =>
                    new
                    {
                        Name = e.DisplayName(),
                        Value = e,
                        ValueName = e.ToString()
                    });

            return new JsonResult(gameEventTypes);
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

        public IActionResult Get(Guid gameId)
        {
            var events = _gameEventService.GetGameEvents(gameId);

            return new JsonResult(events);
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