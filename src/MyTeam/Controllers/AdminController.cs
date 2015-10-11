using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Configuration;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Admin;

namespace MyTeam.Controllers
{
    public class AdminController : BaseController
    {
        [FromServices]
        public IConfiguration Configuration { get; set; }
        [FromServices]
        public IPlayerService PlayerService { get; set; }


        public IActionResult Index()
        {

            var facebookAppId = Configuration["Authentication:Facebook:AppId"];

            var model = new AdminViewModel(facebookAppId);

            return View(model);
        }

        [HttpPost]
        public JsonResult AddPlayer(AddPlayerViewModel model)
        {
            if (ModelState.IsValid)
            {


                PlayerService.Add(Club.ClubId, model.FacebookId, model.FirstName, model.LastName);
                return new JsonResult(new { success = true });
           }
            return new JsonResult(new { success = false });


        }

    }
}  