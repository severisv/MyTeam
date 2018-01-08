using Microsoft.AspNetCore.Mvc;
using MyTeam.Filters;

namespace MyTeam.Controllers
{
    [RequireMember]
    public class NotificationsController : BaseController
    {
        
        public PartialViewResult Index()
        {
            return PartialView("_Index");
        }
    }
}
