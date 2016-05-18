using Microsoft.AspNetCore.Mvc;
using MyTeam.Services.Application;
using MyTeam.ViewModels.Shared;

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
            var notifications = _cacheHelper.GetNotifications(member.Id, HttpContext.GetClub().Id, member.TeamIds);

        var model = new NotificationButtonViewModel
            {
                UnansweredEvents = notifications.UnansweredEvents
        };
            return View("_NotificationButton", model);
        }
      
    }
}