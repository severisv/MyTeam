using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Configuration;
using MyTeam.Filters;
using MyTeam.Models.Enums;
using MyTeam.Models.Structs;
using MyTeam.Resources;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Admin;

namespace MyTeam.Controllers
{
    [RequireMember(Roles.Coach, Roles.Admin)]
    public class AdminController : BaseController
    {
        [FromServices]
        public IConfiguration Configuration { get; set; }
        [FromServices]
        public IPlayerService PlayerService { get; set; }



        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddPlayers()
        {

            var facebookAppId = Configuration["Authentication:Facebook:AppId"];

            var model = new AddPlayersViewModel(facebookAppId);

            return View(model);
        }

        [HttpPost]
        public JsonResult AddPlayer(AddPlayerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = PlayerService.Add(Club.ClubId, model.FacebookId, model.FirstName, model.MiddleName, model.LastName, model.EmailAddress, model.ImageSmall, model.ImageMedium, model.ImageLarge);
                return new JsonResult(response);
           }

            var validationMessage = string.Join(" ,", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return new JsonResult(JsonResponse.ValidationFailed(validationMessage));
        }
        
    }
}  