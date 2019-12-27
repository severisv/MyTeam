using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MyTeam.Filters;
using MyTeam.Models;
using MyTeam.Models.Enums;
using MyTeam.Models.General;
using MyTeam.Models.Shared;
using MyTeam.Models.Structs;
using MyTeam.Resources;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Events;
using NotFoundResult = MyTeam.Extensions.Mvc.NotFoundResult;
using UnauthorizedResult = MyTeam.Extensions.Mvc.UnauthorizedResult;

namespace MyTeam.Controllers
{
    [RequireMember]
    [Route("intern")]
    public class EventController : BaseController
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IEventService _eventService;

        public EventController(ApplicationDbContext dbContext, IEventService eventService)
        {
            _eventService = eventService;
            _dbContext = dbContext;
        }

     
        [RequireMember(Roles.Coach, Roles.Admin)]
        [Route("arrangement/ny")]
        public IActionResult Create(EventType type = EventType.Trening)
        {

            var model = new CreateEventViewModel()
            {
                Type = type,
                Teams = _dbContext.Teams.Where(t => t.ClubId == Club.Id).Select(t => new TeamViewModel
                {
                    Id = t.Id,
                    Name = t.Name
                }).ToList(),
                TeamIds = new List<Guid>()
            };

            ViewBag.Title = Res.CreateEvent;

            return View(model);
        }

        [RequireMember(Roles.Coach, Roles.Admin)]
        [HttpPost]
        [Route("arrangement/ny")]
        public IActionResult Create(CreateEventViewModel model)
        {
            ViewBag.Title = Res.CreateEvent;

            if (ModelState.IsValid)
            {
                var result = new List<EventViewModel>();
                if (model.EventId.HasValue)
                {
                    _eventService.Update(model, Club.Id);
                    result.Add(_eventService.GetEventViewModel(model.EventId.Value));
                }

                else
                {
                    model.ClubId = Club.Id;
                    var events = model.CreateEvents();
                    _eventService.Add(Club.Id, events.ToArray());
                    result.AddRange(events.Select(e => new EventViewModel(e)));
                }

                Alert(AlertType.Success, $"{result.Count} {model.Type.ToString().Pluralize(result.Count).ToLower()} {Res.Saved.ToLower()}");
                return View("CreateSuccess", result);
            }
            return View(model);
        }


        [RequireMember(Roles.Coach, Roles.Admin)]
        [Route("arrangement/endre/{eventId}")]
        public IActionResult Edit(Guid eventId)
        {
            var ev = _eventService.GetEventViewModel(eventId);

            if (ev == null) return new NotFoundResult(HttpContext);

            var model = new CreateEventViewModel(ev)
            {
                Teams = _dbContext.Teams.Where(t => t.ClubId == Club.Id).Select(t => new TeamViewModel
                {
                    Id = t.Id,
                    Name = t.Name
                }).ToList()
            };

            ViewBag.Title = $"{Res.Edit} {ev.Type.ToString().ToLower()}";

            return View("Create", model);
        }

        [RequireMember(Roles.Coach, Roles.Admin)]
        [Route("arrangement/slett")]
        public IActionResult Delete(Guid eventId)
        {
            var ev = _eventService.Get(eventId);

            if (ev == null) return new NotFoundResult(HttpContext);

            _eventService.Delete(Club.Id, eventId);

            Alert(AlertType.Success, $"{ev.Type.FromInt()} {Res.Deleted.ToLower()}");
            if (HttpContext.Request.IsAjaxRequest()) return PartialView("_Alerts");
            return RedirectToRoute("/intern");
        }

    }
}
