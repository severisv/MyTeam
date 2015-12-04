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
    [Route("spillere")]
    public class PlayerController : BaseController
    {
        [FromServices]
        public IRepository<Player> PlayerRepository { get; set; }
        [FromServices]
        public IPlayerService PlayerService { get; set; }

        [Route("oversikt")]
        public IActionResult List(PlayerStatus type = PlayerStatus.Aktiv, Guid? playerId = null, bool editMode = false)
        {
            var players = PlayerRepository.Get().Where(p => p.Status == type && p.Club.ClubIdentifier == HttpContext.GetClub().ClubId);

            var model = new ShowPlayersViewModel(players, editMode, type);
            model.SelectedPlayerId = playerId;

            ViewBag.PageName = model.SelectedPlayer != null ?
                model.SelectedPlayer.Name: 
                Res.PlayersOfType(type);


            return View("List",model);
        }

        [Route("vis")]
        public IActionResult Show(Guid playerId)
        {
            if (Request.IsAjaxRequest())
            {
                var player = PlayerRepository.GetSingle(playerId);
                return PartialView("_Show", player);
            }
            return List(playerId: playerId);           
           
        }

        [Route("endre")]
        [RequireMember(true, Roles.Admin, Roles.Coach)]
        public IActionResult Edit(Guid playerId)
        {
            if (Request.IsAjaxRequest())
            {
                var player = PlayerRepository.GetSingle(playerId);
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
                PlayerService.EditPlayer(model);
                return Show(model.Id);

            }
            return PartialView("_Edit", model);
        }
      

    
    }
}
