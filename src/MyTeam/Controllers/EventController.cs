using System;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using MyTeam.Filters;
using MyTeam.Models.Enums;
using MyTeam.Resources;
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

        [RequirePlayer]
        [ValidateModelState]
        public IActionResult Signup(Guid eventId, bool isAttending)
        {
            var ev = EventService.Get(eventId);

            if (isAttending == false && ev.SignoffHasClosed())
            {
                ViewBag.SignupMessage = Res.SignoffClosed;
            }
            else if (ev.SignupHasClosed())
            {
                return new UnauthorizedResult(Request.HttpContext);
            }
            else
            {
                EventService.SetAttendance(ev, CurrentPlayer.Id, isAttending);
            }

            return PartialView("_Signup", ev);
        }
       
        //[Authorize(Roles = Roles.Coach)]
        public IActionResult Create()
        {
            var model = new CreateEventViewModel();
            return View(model);
        }

        //[Authorize(Roles = Roles.Coach)]
        [HttpPost]
        public IActionResult Create(CreateEventViewModel model)
        {
            
            return View();
        }

    }
}
