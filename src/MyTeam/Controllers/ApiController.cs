using System;
using System.Linq;
using Microsoft.AspNet.Mvc;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
using MyTeam.Models.Structs;
using MyTeam.Resources;
using MyTeam.Services.Domain;
using MyTeam.Services.Repositories;
using MyTeam.ViewModels.Player;


namespace MyTeam.Controllers
{
    public class ApiController : BaseController
    {
        [FromServices]
        public IPlayerService PlayerService { get; set; }
        
        public JsonResult GetPlayers()
        {
            var players = PlayerService.Get(Club.ClubId);
            return new JsonResult(players);
        }

        public JsonResult GetFacebookIds()
        {
            var ids = PlayerService.GetFacebookIds();
            return new JsonResult(new { data = ids });
        }

        [HttpPost]
        public JsonResult SetPlayerStatus(Guid id, PlayerStatus status)
        {
            if (ModelState.IsValid)
            {
                var reponse = new {Success = true};
                return new JsonResult(reponse);
            }

            var validationMessage = string.Join(" ,", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return new JsonResult(JsonResponse.ValidationFailed(validationMessage));
        }

    }
}
