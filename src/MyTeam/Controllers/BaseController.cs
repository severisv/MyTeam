using Microsoft.AspNet.Mvc;
using MyTeam.Filters;
using MyTeam.Models.Enums;
using MyTeam.Services.Application;

namespace MyTeam.Controllers
{
    //[HandleError]
    public class BaseController : Controller
    {
        [FromServices]
        public ICacheHelper CacheHelper { get; set; }

        public virtual CurrentClub Club => HttpContext.GetClub();
        public virtual UserMember CurrentMember => HttpContext.Member();

  
        [NonAction]
        public void Alert(AlertType type, string message)
        {
            if (Request.IsAjaxRequest())
            {
                ViewData.Add($"AjaxAlert{type}", message);
            }
            else
            {
                ViewData.Add($"Alert{type}", message);
            }

        }
     
    }
}
