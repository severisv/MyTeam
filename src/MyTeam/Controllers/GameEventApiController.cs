using System;
using System.Linq;
using System.Threading;
using Microsoft.AspNet.Mvc;
using MyTeam.Filters;
using MyTeam.Models.Enums;
using MyTeam.Models.Structs;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Game;

namespace MyTeam.Controllers
{
    public class GameEventApiController : BaseController
    {
        [FromServices]
        public IGameEventService GameEventService { get; set; }

        
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
                var gameEvent = GameEventService.AddGameEvent(model);
                return new JsonResult(gameEvent);
            }
            return new JsonResult(JsonResponse.Failure);
        }

        public IActionResult Get(Guid gameId)
        {
            var events = GameEventService.GetGameEvents(gameId);

            return new JsonResult(events);
        }


        [HttpPost]
        [RequireMember(Roles.Admin, Roles.Coach, Roles.NewsWriter)]
        public IActionResult Delete(Guid eventId)
        {
            if (ModelState.IsValid)
            {
                GameEventService.Delete(eventId);
                return new JsonResult(JsonResponse.Success());
            }
            return new JsonResult(JsonResponse.Failure);
        }
    }
}