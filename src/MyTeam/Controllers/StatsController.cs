using System.Linq;
using Microsoft.AspNet.Mvc;
using MyTeam.Models.Enums;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Table;

namespace MyTeam.Controllers
{
    [Route("statistikk")]
    public class StatsController : BaseController
    {
        [FromServices]
        public IStatsService StatsService { get; set; }

        public IActionResult Index()
        {
            Alert(AlertType.Info, "Det er ikke registrert noe statistikk enda");
            return View();
        }
        

    }
}