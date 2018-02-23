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
using MyTeam.ViewModels.Table;
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
        private readonly IPlayerService _playerService;

        public EventController(ApplicationDbContext dbContext, IEventService eventService, IPlayerService playerService)
        {
            _playerService = playerService;
            _eventService = eventService;
            _dbContext = dbContext;
        }

        [Route("arrangementer")]
        public IActionResult IndexRedirect()
        {
            return RedirectToAction("Index");
        }

        [Route("")]
        public IActionResult Index(EventType type = EventType.Alle, bool showAll = false)
        {
            var teamIds = CurrentMember.Roles.ContainsAny(Roles.Admin, Roles.Coach)
              ? Club.TeamIds
              : CurrentMember.TeamIds;

            var events = _eventService.GetUpcoming(type, Club.Id, teamIds, showAll);

            if (Request.IsAjaxRequest())
            {
                events = new PagedList<EventViewModel>(events.Where(e => !e.SignupHasOpened()), events.Skip, events.Take, events.TotalCount);
                return PartialView("_ListEvents", events);
            }
            var model = new UpcomingEventsViewModel(events, type, previous: false);
            return View("Index", model);
        }


        [Route("arrangementer/tidligere/{type?}/{page:int?}")]
        public IActionResult Previous(EventType type = EventType.Alle, int page = 1)
        {
            var teamIds = CurrentMember.Roles.ContainsAny(Roles.Admin, Roles.Coach)
                ? Club.TeamIds
                : CurrentMember.TeamIds;

            var events = _eventService.GetPrevious(teamIds, type, page);

            var model = new UpcomingEventsViewModel(events, type, previous: true);
            return View("Index", model);
        }


        [Route("prev")]
        public IActionResult Prev(EventType type = EventType.Alle, int page = 1)
        {
            var teamIds = CurrentMember.Roles.ContainsAny(Roles.Admin, Roles.Coach)
                ? Club.TeamIds
                : CurrentMember.TeamIds;

            var events = _eventService.GetPrevious(teamIds, type, page);

            return new JsonResult(events);
        }


        [ValidateModelState]
        [Route("pamelding")]
        public IActionResult Signup(Guid eventId, bool isAttending)
        {
            var ev = _eventService.GetSignupDetailsViewModel(eventId);

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
                var attendee = _eventService.SetAttendance(ev.Id, CurrentMember.Id, isAttending, Club.Id);
                ev.SetAttendance(attendee, isAttending);
            }
            return PartialView("_SignupDetails", ev);
        }

        [Route("paameldte")]
        public IActionResult SignupDetails(Guid eventId)
            => PartialView("_SignupDetails", _eventService.GetSignupDetailsViewModel(eventId));

        [HttpPost]
        [RequireMember]
        [Route("paamelding/beskjed")]
        public JsonResult SignupMessage(Guid eventId, string message)
        {
            if (ModelState.IsValid)
            {
                var reponse = new { Success = true };
                _eventService.SignupMessage(eventId, CurrentMember.Id, message);
                return new JsonResult(reponse);
            }

            var validationMessage = string.Join(" ,", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return new JsonResult(JsonResponse.ValidationFailed(validationMessage));
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
        [Route("arrangement/endre")]
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

            Alert(AlertType.Success, $"{ev.EventType} {Res.Deleted.ToLower()}");
            if (HttpContext.Request.IsAjaxRequest()) return PartialView("_Alerts");
            return RedirectToAction("Index");
        }

        [RequireMember(Roles.Coach, Roles.Admin, Roles.AttendanceTaker)]
        [Route("oppmote/bekreft")]
        public IActionResult RegisterAttendance(Guid? eventId = null)
        {
            var ev = _eventService.GetRegisterAttendanceEventViewModel(eventId);
            var players = _playerService.GetDto(Club.Id);
            var previousEvents = _eventService.GetPreviousSimpleEvents(EventType.Trening, Club.Id, 15).ToList();

            if (ev == null) return new NotFoundResult(HttpContext);

            var model = new RegisterAttendanceViewModel(ev, players, previousEvents);

            ViewBag.Title = $"{Res.Register} {Res.Attendance.ToLower()}";

            return View("RegisterAttendance", model);
        }

        [HttpPost]
        [RequireMember(Roles.Coach, Roles.Admin, Roles.AttendanceTaker)]
        [Route("oppmote/bekreft")]
        public JsonResult ConfirmAttendance(Guid eventId, Guid playerId, bool didAttend)
        {
            if (eventId == Guid.Empty || playerId == Guid.Empty) return new JsonResult(JsonResponse.ValidationFailed("EventId eller PlayerId er null"));

            _eventService.ConfirmAttendance(eventId, playerId, didAttend);

            return new JsonResult(JsonResponse.Success());
        }


    }
}
