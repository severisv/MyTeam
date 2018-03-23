using Microsoft.AspNetCore.Mvc;
using MyTeam.Filters;
using MyTeam.Models.Enums;

namespace MyTeam.Controllers
{
    public class BaseController : Controller
    {
       
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
