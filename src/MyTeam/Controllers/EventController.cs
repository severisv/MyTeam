using System;
using Microsoft.AspNet.Mvc;
using MyTeam.Models.Enums;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Events;


namespace MyTeam.Controllers
{
    public class EventController : BaseController
    {
        [FromServices]
        public IEventService EventService { get; set; }


        public IActionResult Index(EventType type = EventType.Alle)
        {
            var events = EventService.GetUpcoming(type);

            var model = new UpcomingEventsViewModel(events, type);

            return View(model);
        }

        public IActionResult Signup(Guid eventId, bool isAttending)
        {
            var ev = EventService.SetAttendanceReturnsEvent(ActivePlayer.Id, eventId, isAttending);

            return PartialView("_Signup", ev);
        }

       

    
    }
}
