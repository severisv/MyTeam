using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Configuration;
using MyTeam.ViewModels.Admin;

namespace MyTeam.Controllers
{
    public class AdminController : BaseController
    {
        [FromServices]
        public IConfiguration Configuration { get; set; }


        public IActionResult Index()
        {

            var facebookAppId = Configuration["Authentication:Facebook:AppId"];

            var model = new AdminViewModel(facebookAppId);

            return View(model);
        }
        
    }
}  