﻿using System;
using System.Reflection;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using MyTeam.Filters;
using MyTeam.Models.Domain;
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
            
            return View("Index", model);
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

            return PartialView("_SignupDetails", ev);
        }
       
        //[Authorize(Roles = Roles.Coach)]
        public IActionResult Create(EventType type = EventType.Trening)
        {
            var model = new CreateEventViewModel()
            {
                Type = type
            };

            ViewBag.Title = Res.CreateEvent;

            return View(model);
        }

        //[Authorize(Roles = Roles.Coach)]
        [HttpPost]
        public IActionResult Create(CreateEventViewModel model)
        {
            ViewBag.Title = Res.CreateEvent;

            if (ModelState.IsValid)
            {
                Event ev;

                if (model.EventId.HasValue)
                {
                    ev = EventService.Get(model.EventId.Value);
                    model.UpdateEvent(ev);
                    EventService.Update(ev);
                }

                else
                {
                    ev = model.CreateEvent();
                    EventService.Add(ev);
                }
                
                Alert(AlertType.Success, $"{ev.Type} {Res.Saved.ToLower()}");
                return View("CreateSuccess", ev);
            }
            return View(model);
        }
        
        
        //[Authorize(Roles = Roles.Coach)]
        public IActionResult Edit(Guid eventId)
        {
            var ev = EventService.Get(eventId);

            if (ev == null) return new NotFoundResult(Context);

            var model = new CreateEventViewModel(ev);

            ViewBag.Title = $"{Res.Edit} {ev.Type.ToString().ToLower()}";

            return View("Create", model);
        }

        public IActionResult Delete(Guid eventId)
        {
            var ev = EventService.Get(eventId);

            if (ev == null) return new NotFoundResult(Context);

            EventService.Delete(eventId);
            Alert(AlertType.Success, $"{ev.Type} {Res.Deleted.ToLower()}");
            return Index();
        }

      
    }
}
