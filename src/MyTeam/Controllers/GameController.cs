using System;
using System.Linq;
using Microsoft.AspNet.Mvc;
using MyTeam.Filters;
using MyTeam.Models.Enums;
using MyTeam.Models.Structs;
using MyTeam.Resources;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Attendance;
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

        [Route("")]
        public IActionResult Index()
        {
            Alert(AlertType.Info, "Det er ikke registrert noen kamper enda");
            return View();
        }


        [RequireMember(Roles.Coach, Roles.Admin)]
        [Route("laguttak")]
        public IActionResult RegisterSquad(Guid eventId)
        {
            var ev = EventService.GetRegisterAttendanceEventViewModel(eventId);

            var players = PlayerService.GetDto(Club.ClubId);

            if (ev == null) return new NotFoundResult(HttpContext);

            var model = new RegisterSquadViewModel(ev, players);

            ViewBag.Title = "Laguttak";

            return View("RegisterSquad", model);
        }

        [HttpPost]
        [RequireMember(Roles.Coach, Roles.Admin)]
        [Route("oppmote/bekreft")]
        public JsonResult SelectPlayer(Guid eventId, Guid playerId, bool isSelected)
        {
            if (eventId == Guid.Empty || playerId == Guid.Empty) return new JsonResult(JsonResponse.ValidationFailed("EventId eller PlayerId er null"));


            return new JsonResult(JsonResponse.Success());
        }

    }
}