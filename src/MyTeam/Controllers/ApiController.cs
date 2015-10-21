using System;
using System.Linq;
using Microsoft.AspNet.Mvc;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
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

    }
}
