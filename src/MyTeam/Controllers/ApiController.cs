using System;
using System.Linq;
using Microsoft.AspNet.Mvc;
using MyTeam.Filters;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
using MyTeam.Models.Structs;
using MyTeam.Resources;
using MyTeam.Services.Domain;
using MyTeam.Services.Repositories;
using MyTeam.ViewModels.Player;


namespace MyTeam.Controllers
{
    [RequireMember]
    public class ApiController : BaseController
    {
        [FromServices]
        public IPlayerService PlayerService { get; set; }
        
        public JsonResult GetPlayers()
        {
            var players = PlayerService.Get(Club.ClubId);
            return new JsonResult(players);
        }

        [RequireMember(Roles.Coach, Roles.Admin)]
        public JsonResult GetFacebookIds()
        {
            var ids = PlayerService.GetFacebookIds();
            return new JsonResult(new { data = ids });
        }

        [HttpPost]
        [RequireMember(Roles.Coach, Roles.Admin)]
        public JsonResult SetPlayerStatus(Guid id, PlayerStatus status)
        {
            if (ModelState.IsValid)
            {
                PlayerService.SetPlayerStatus(id, status);
                var reponse = new {Success = true};
                return new JsonResult(reponse);
            }

            var validationMessage = string.Join(" ,", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return new JsonResult(JsonResponse.ValidationFailed(validationMessage));
        }

        [HttpPost]
        [RequireMember(Roles.Coach, Roles.Admin)]
        public JsonResult TogglePlayerRole(Guid id, string role)
        {
            if (ModelState.IsValid)
            {
                PlayerService.TogglePlayerRole(id, role);
                var reponse = new {Success = true};
                return new JsonResult(reponse);
            }

            var validationMessage = string.Join(" ,", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return new JsonResult(JsonResponse.ValidationFailed(validationMessage));
        }

    }
}
