using System;
using Microsoft.AspNet.Mvc;
using MyTeam.Filters;
using MyTeam.Models;
using MyTeam.Models.Enums;
using MyTeam.Resources;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Player;


namespace MyTeam.Controllers
{
    [Route("spillere")]
    public class PlayerController : BaseController
    {
        [FromServices]
        public ApplicationDbContext DbContext { get; set; }
        [FromServices]
        public IPlayerService PlayerService { get; set; }

        [Route("")]
        public IActionResult List(PlayerStatus type = PlayerStatus.Aktiv)
        {
            var players = PlayerService.GetPlayers(type, Club.Id);
            
            var model = new ShowPlayersViewModel(players, type);

            ViewBag.PageName = Res.PlayersOfType(type);

            return View("List",model);
        }

        [Route("vis/{playerId?}")]
        public IActionResult Show(Guid playerId)
        {
            var selectedPlayer = PlayerService.GetSingle(playerId);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_Show", selectedPlayer);
            }

            var players = PlayerService.GetPlayers(selectedPlayer.Status, Club.Id);
            var model = new ShowPlayersViewModel(players, selectedPlayer.Status, selectedPlayer);
            return View("Show", model);  
        }


        [Route("stats")]
        public IActionResult GetStats(Guid playerId)
        {
            var model = PlayerService.GetStats(playerId, Club.TeamIds);
            return PartialView("_GetStats", model);
        }


        [Route("endre")]
        [RequireMember(true, Roles.Admin, Roles.Coach)]
        public IActionResult Edit(Guid playerId, bool filterRedirect = false)
        {
            if(filterRedirect)
                Alert(AlertType.Info, "Vennligst fullfør spillerprofilen din");

            var selectedPlayer = PlayerService.GetSingle(playerId);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_Edit", new EditPlayerViewModel(selectedPlayer));
            }

            var players = PlayerService.GetPlayers(selectedPlayer.Status, Club.Id);
            var model = new ShowPlayersViewModel(players, selectedPlayer.Status, selectedPlayer);
            return View("Edit", model);

        }


        [HttpPost]
        [RequireMember(true, Roles.Admin, Roles.Coach)]
        [Route("endre")]
        public IActionResult Edit(EditPlayerViewModel model)
        {
            if (ModelState.IsValid)
            {
                PlayerService.EditPlayer(model, Club.ClubId);
                Alert(AlertType.Success, "Profil lagret");
                return Show(model.PlayerId);

            }
            return PartialView("_Edit", model);
        }
    
    }
}
