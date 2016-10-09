using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MyTeam.Filters;
using MyTeam.Models.Enums;
using MyTeam.Models.Structs;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Admin;

namespace MyTeam.Controllers
{
    [Route("admin")]
    [RequireMember(Roles.Coach, Roles.Admin)]
    public class AdminController : BaseController
    {
        private readonly IPlayerService _playerService;

        public AdminController(IPlayerService playerService)
        {
            _playerService = playerService;
        }


        public IActionResult Index()
        {
            return View();
        }

        [Route("spillerinvitasjon")]
        public IActionResult AddPlayers()
        {
            return View();
        }

        [HttpPost]
        [Route("spillerinvitasjon")]
        public JsonResult AddPlayer(AddPlayerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = _playerService.Add(Club.ClubId, model.FacebookId, model.FirstName, model.MiddleName, model.LastName, model.EmailAddress);
                return new JsonResult(response);
           }

            var validationMessage = string.Join(" ,", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return new JsonResult(JsonResponse.ValidationFailed(validationMessage));
        }
        
    }
}  