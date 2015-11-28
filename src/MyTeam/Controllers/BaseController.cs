using Microsoft.AspNet.Mvc;
using MyTeam.Filters;
using MyTeam.Models.Enums;
using MyTeam.Services.Application;

namespace MyTeam.Controllers
{
    [HandleError]
    public class BaseController : Controller
    {
        [FromServices]
        public ICacheHelper CacheHelper { get; set; }

        public virtual CurrentClub Club => HttpContext.GetClub();
        public virtual UserMember CurrentMember => HttpContext.Member();

  
        public void Alert(AlertType type, string message)
        {
            ViewData.Add($"Alert{type}", message);
        }
     
    }
}
