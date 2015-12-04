﻿using System;
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
    [Route("intern/arrangementer")]
    public class EventController : BaseController
    {
        [FromServices]
        public IEventService EventService { get; set; }
        [FromServices]
        public IPlayerService PlayerService { get; set; }


        public IActionResult Index(EventType type = EventType.Alle, bool previous = false)
        {
            var events = previous
                ? EventService.GetPrevious(type, Club.Id)
                : EventService.GetUpcoming(type, Club.Id);


            var model = new UpcomingEventsViewModel(events, type, previous);

            return View("Index", model);
        }

        [ValidateModelState]
        [Route("pamelding")]
        public IActionResult Signup(Guid eventId, bool isAttending)
        {
            var ev = EventService.GetEventViewModel(eventId);

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
                EventService.SetAttendance(ev.Id, CurrentMember.Id, isAttending);
                ev.SetAttendance(CurrentMember.Id, isAttending);
            }
            return PartialView("_SignupDetails", ev);
        }

        [RequireMember(Roles.Coach, Roles.Admin)]
        [Route("ny")]
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
        [Route("ny")]
        public IActionResult Create(CreateEventViewModel model)
        {
            ViewBag.Title = Res.CreateEvent;

            if (ModelState.IsValid)
            {
                var result = new List<EventViewModel>();
                if (model.EventId.HasValue)
                {
                    EventService.Update(model);
                    result.Add(EventService.GetEventViewModel(model.EventId.Value));
                }

                else
                {
                    model.ClubId = HttpContext.GetClub().Id;
                    var events = model.CreateEvents();
                    EventService.Add(events.ToArray());

                    result.AddRange(events.Select(e => new EventViewModel(e)));
                }

                Alert(AlertType.Success, $"{result.Count} {model.Type.ToString().Pluralize(result.Count).ToLower()} {Res.Saved.ToLower()}");
                return View("CreateSuccess", result);
            }
            return View(model);
        }


        [RequireMember(Roles.Coach, Roles.Admin)]
        [Route("endre")]
        public IActionResult Edit(Guid eventId)
        {
            var ev = EventService.GetEventViewModel(eventId);

            if (ev == null) return new NotFoundResult(HttpContext);

            var model = new CreateEventViewModel(ev);

            ViewBag.Title = $"{Res.Edit} {ev.Type.ToString().ToLower()}";

            return View("Create", model);
        }

        [RequireMember(Roles.Coach, Roles.Admin)]
        [Route("slett")]
        public IActionResult Delete(Guid eventId)
        {
            var ev = EventService.Get(eventId);

            if (ev == null) return new NotFoundResult(HttpContext);

            EventService.Delete(eventId);

            Alert(AlertType.Success, $"{ev.Type} {Res.Deleted.ToLower()}");
            if (HttpContext.Request.IsAjaxRequest()) return PartialView("_Alerts");
            return Index();
        }

        [RequireMember(Roles.Coach, Roles.Admin)]
        [Route("oppmote/bekreft")]
        public IActionResult RegisterAttendance(Guid eventId)
        {
            var ev = EventService.GetEventViewModel(eventId);
            var players = PlayerService.GetDto(HttpContext.GetClub().ClubId);
            var previousEvents = EventService.GetPrevious(EventType.Trening, Club.Id, 15).ToList();

            if (ev == null) return new NotFoundResult(HttpContext);

            var model = new RegisterAttendanceViewModel(ev, players, previousEvents);

            ViewBag.Title = $"{Res.Register} {Res.Attendance.ToLower()}";

            return View("RegisterAttendance", model);
        }

        [HttpPost]
        [RequireMember(Roles.Coach, Roles.Admin)]
        [Route("oppmote/bekreft")]
        public JsonResult ConfirmAttendance(Guid eventId, Guid playerId, bool didAttend)
        {
            if (eventId == Guid.Empty || playerId == Guid.Empty) return new JsonResult(JsonResponse.ValidationFailed("EventId eller PlayerId er null"));

            EventService.ConfirmAttendance(eventId, playerId, didAttend);

            return new JsonResult(JsonResponse.Success());
        }
    }
}
