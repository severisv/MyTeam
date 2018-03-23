using Microsoft.AspNetCore.Mvc;
using MyTeam.Services.Application;

namespace MyTeam.ViewComponents.Shared
{
    public class NotificationButtonViewComponent : ViewComponent
    {
        private readonly ICacheHelper _cacheHelper;

        public NotificationButtonViewComponent(ICacheHelper cacheHelper)
        {
            _cacheHelper = cacheHelper;
        }

        public IViewComponentResult Invoke()
        {
            var member = HttpContext.Member();
            var model = _cacheHelper.GetNotifications(member.Id, HttpContext.GetClub().Id, member.TeamIds);
            
            return View("_NotificationButton", model);
        }
      
    }
}