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
    public class PlayerController : BaseController
    {
        [FromServices]
        public IRepository<Player> PlayerRepository { get; set; }
        [FromServices]
        public IPlayerService PlayerService { get; set; }


        public IActionResult Index(PlayerStatus type = PlayerStatus.Aktiv, Guid? playerId = null, bool editMode = false)
        {
            var players = PlayerRepository.Get().Where(p => p.Status == type);

            var model = new ShowPlayersViewModel(players, editMode)
            {
                SelectedPlayerId = playerId
            };

            ViewBag.PageName = model.SelectedPlayer != null ?
                model.SelectedPlayer.Name: 
                Res.PlayersOfType(type);


            return View("Index",model);
        }
    

        public IActionResult Show(Guid playerId)
        {
            if (Request.IsAjaxRequest())
            {
                var player = PlayerRepository.GetSingle(playerId);
                return PartialView("_Show", player);
            }
            return Index(playerId: playerId);           
           
        }

        public IActionResult Edit(Guid playerId)
        {
            if (Request.IsAjaxRequest())
            {
                var player = PlayerRepository.GetSingle(playerId);
                return PartialView("_Edit", new EditPlayerViewModel(player));
            }
            return Index(playerId: playerId, editMode: true);

        }


        [HttpPost]
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
