using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using MyTeam.Filters;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
using MyTeam.Models.Structs;
using MyTeam.Resources;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Events;


namespace MyTeam.Controllers
{
    [RequireMember]
    public class EventController : BaseController
    {
        [FromServices]
        public IEventService EventService { get; set; }
        [FromServices]
        public IPlayerService PlayerService { get; set; }


        public IActionResult Index(EventType type = EventType.Alle, bool previous = false)
        {
            var events = previous
                ? EventService.GetPrevious(type)
                : EventService.GetUpcoming(type);


            var model = new UpcomingEventsViewModel(events, type, previous);

            return View("Index", model);
        }

        [RequireMember(true, Roles.DenyAll)]
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
                EventService.SetAttendance(ev, CurrentMember.Id, isAttending);
            }

            return PartialView("_SignupDetails", ev);
        }

        [RequireMember(Roles.Coach, Roles.Admin)]
        public IActionResult Create(EventType type = EventType.Trening)
        {
            var model = new CreateEventViewModel()
            {
                Type = type
            };

            ViewBag.Title = Res.CreateEvent;

            return View(model);
        }

        [RequireMember(Roles.Coach, Roles.Admin)]
        [HttpPost]
        public IActionResult Create(CreateEventViewModel model)
        {
            ViewBag.Title = Res.CreateEvent;

            if (ModelState.IsValid)
            {
                var result = new List<Event>();

                if (model.EventId.HasValue)
                {
                    var ev = EventService.Get(model.EventId.Value);
                    model.UpdateEvent(ev);
                    EventService.Update(ev);
                    result.Add(ev);
                }

                else
                {
                    var events = model.CreateEvents();
                    result.AddRange(events);
                    EventService.Add(events.ToArray());

                }

                Alert(AlertType.Success, $"{result.Count} {model.Type.ToString().Pluralize(result.Count).ToLower()} {Res.Saved.ToLower()}");
                return View("CreateSuccess", result);
            }
            return View(model);
        }


        [RequireMember(Roles.Coach, Roles.Admin)]
        public IActionResult Edit(Guid eventId)
        {
            var ev = EventService.Get(eventId);

            if (ev == null) return new NotFoundResult(Context);

            var model = new CreateEventViewModel(ev);

            ViewBag.Title = $"{Res.Edit} {ev.Type.ToString().ToLower()}";

            return View("Create", model);
        }

        [RequireMember(Roles.Coach, Roles.Admin)]
        public IActionResult Delete(Guid eventId)
        {
            var ev = EventService.Get(eventId);

            if (ev == null) return new NotFoundResult(Context);

            EventService.Delete(eventId);

            Alert(AlertType.Success, $"{ev.Type} {Res.Deleted.ToLower()}");
            if (Context.Request.IsAjaxRequest()) return PartialView("_Alerts");
            return Index();
        }

        [RequireMember(Roles.Coach, Roles.Admin)]
        public IActionResult RegisterAttendance(Guid eventId)
        {
            var ev = EventService.Get(eventId) as Training;
            var players = PlayerService.GetDto(Context.GetClub().ClubId);

            if (ev == null) return new NotFoundResult(Context);

            var model = new RegisterAttendanceViewModel(ev, players);

            ViewBag.Title = $"{Res.Register} {Res.Attendance.ToLower()}";

            return View("RegisterAttendance", model);
        }

        [HttpPost]
        [RequireMember(Roles.Coach, Roles.Admin)]
        public JsonResult ConfirmAttendance(Guid attendanceId, bool didAttend)
        {
            EventService.ConfirmAttendance(attendanceId, didAttend);

            return new JsonResult(JsonResponse.Success());
        }
    }
}
