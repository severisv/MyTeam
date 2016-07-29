using System;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IPlayerService _playerService;

        public PlayerController(IPlayerService playerService)
        {
            _playerService = playerService;
        }


        [Route("")]
        public IActionResult List(PlayerStatus type = PlayerStatus.Aktiv)
        {
            var players = _playerService.GetPlayers(type, Club.Id);
            
            var model = new ShowPlayersViewModel(players, type);

            ViewBag.PageName = Res.PlayersOfType(type);

            return View("List",model);
        }

        [Route("vis/{name?}")]
        public IActionResult Show(string name)
        {
            var selectedPlayer = _playerService.GetSingle(name);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_Show", selectedPlayer);
            }

            var players = _playerService.GetPlayers(selectedPlayer.Status, Club.Id);
            var model = new ShowPlayersViewModel(players, selectedPlayer.Status, selectedPlayer);
            return View("Show", model);  
        }


        [Route("stats")]
        public IActionResult GetStats(Guid playerId)
        {
            var model = _playerService.GetStats(playerId, Club.TeamIds);
            return PartialView("_GetStats", model);
        }


        [Route("endre/{name}")]
        [RequireMember(true, Roles.Admin, Roles.Coach)]
        public IActionResult Edit(string name, bool filterRedirect = false)
        {
            if(filterRedirect)
                Alert(AlertType.Info, "Vennligst fullfør spillerprofilen din");

            var selectedPlayer = _playerService.GetSingle(name);

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
        [Route("endre")]
        public IActionResult Edit(EditPlayerViewModel model)
        {
            if (ModelState.IsValid)
            {
                _playerService.EditPlayer(model, Club.ClubId);
                Alert(AlertType.Success, "Profil lagret");
                return Show(model.UrlName);

            }
            return PartialView("_Edit", model);
        }
    
    }
}
