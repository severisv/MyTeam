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
        public IActionResult List(PlayerStatus type = PlayerStatus.Aktiv, Guid? playerId = null, bool editMode = false)
        {
            var players = PlayerService.GetPlayers(type, Club.Id);

            var model = new ShowPlayersViewModel(players, editMode, type)
            {
                SelectedPlayerId = playerId
            };

            ViewBag.PageName = model.SelectedPlayer != null ?
                model.SelectedPlayer.Fullname: 
                Res.PlayersOfType(type);

            return View("List",model);
        }

        [Route("vis")]
        public IActionResult Show(Guid playerId)
        {
            if (Request.IsAjaxRequest())
            {
                var model = PlayerService.GetSingle(playerId);
                return PartialView("_Show", model);
            }
            return List(playerId: playerId);           
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

            if (Request.IsAjaxRequest())
            {
                var player = PlayerService.GetSingle(playerId);
                return PartialView("_Edit", new EditPlayerViewModel(player));
            }
            return List(playerId: playerId, editMode: true);

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
