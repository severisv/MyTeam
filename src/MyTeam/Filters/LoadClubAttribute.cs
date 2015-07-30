using Microsoft.AspNet.Mvc;
using MyTeam.Controllers;

namespace MyTeam.Filters
{
    public class LoadClubAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            var controller = actionContext.Controller as BaseController;
            if (controller == null) return;

            var memoryStore = controller.MemoryStore;
            var clubId = actionContext.RouteData.Values["club"] as string;
            var club = memoryStore.GetClub(clubId);

            actionContext.HttpContext.Items["club"] = club;
        }
    }
}