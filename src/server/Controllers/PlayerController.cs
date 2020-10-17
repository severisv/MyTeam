using System;
using Microsoft.AspNetCore.Mvc;
using MyTeam.Filters;
using MyTeam.Models;
using MyTeam.Models.Enums;
using MyTeam.Resources;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Player;
using NotFoundResult = MyTeam.Extensions.Mvc.NotFoundResult;



namespace MyTeam.Controllers
{
    public class PlayerController : BaseController
    {
        private readonly IPlayerService _playerService;

        public PlayerController(IPlayerService playerService)
        {
            _playerService = playerService;
        }


        [Route("spillere/endre")]
        [RequireMember(true, Roles.Admin, Roles.Coach)]
        public IActionResult Edit(Guid playerId, bool filterRedirect = false)
        {
        
            if (filterRedirect)
                Alert(AlertType.Info, "Vennligst fullfør spillerprofilen din");

            var selectedPlayer = _playerService.GetSingle(playerId);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_Edit", new EditPlayerViewModel(selectedPlayer));
            }

            var players = _playerService.GetPlayers(selectedPlayer.Status, Club.Id);
            var model = new ShowPlayersViewModel(players, selectedPlayer.Status, selectedPlayer);
            return View("Edit", model);

        }


        [HttpPost]
        [RequireMember(true, Roles.Admin, Roles.Coach)]
        [Route("spillere/endre")]
        public IActionResult Edit(EditPlayerViewModel model)
        {
            if (ModelState.IsValid)
            {
                _playerService.EditPlayer(model, Club.Id);
                Alert(AlertType.Success, "Profil lagret");
                return Redirect($"/spillere/vis/{model.UrlName}");

            }
            return PartialView("_Edit", model);
        }

    }
}
