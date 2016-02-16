using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNet.Mvc;
using MyTeam.Filters;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
using MyTeam.Models.Structs;
using MyTeam.Services.Domain;

namespace MyTeam.Controllers
{
    public class GameApiController : BaseController
    {
        [FromServices]
        public IGameService GameService { get; set; }


        [HttpPost]
        [RequireMember(Roles.Admin, Roles.Coach)]
        public IActionResult SetHomeScore(Guid gameId, int? value)
        {
            if (ModelState.IsValid)
            {
                GameService.SetHomeScore(gameId, value);
                return new JsonResult(JsonResponse.Success());
            }
            return new JsonResult(JsonResponse.Failure);
        }

        [HttpPost]
        [RequireMember(Roles.Admin, Roles.Coach)]
        public IActionResult SetAwayScore(Guid gameId, int? value)
        {
            if (ModelState.IsValid)
            {
                GameService.SetAwayScore(gameId, value);
                return new JsonResult(JsonResponse.Success());
            }
            return new JsonResult(JsonResponse.Failure);
        }


     
        public IActionResult GetEventTypes()
        {
            var asd = GameEventType.Goal.DisplayName();

            var gameEventTypes = Enum.GetValues(typeof (GameEventType)).Cast<GameEventType>().Select(e =>
                new
                {
                    Name = e.DisplayName(),
                    Value = e.ToString()
                });

         
            return new JsonResult(gameEventTypes);
        }
    }
}