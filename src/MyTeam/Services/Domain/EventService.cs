using System;
using System.Collections.Generic;
using System.Linq;
using MyTeam.Models;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
using MyTeam.ViewModels.Events;
using Microsoft.Data.Entity;
using MyTeam.Services.Application;

namespace MyTeam.Services.Domain
{
    public class EventService : IEventService
    {

        private readonly ApplicationDbContext _dbContext;
        private readonly ICacheHelper _cacheHelper;

        public EventService(ApplicationDbContext dbContext, ICacheHelper cacheHelper)
        {
            _dbContext = dbContext;
            _cacheHelper = cacheHelper;
        }

        public Event Get(Guid id)
        {
            return _dbContext.Events.Single(e => e.Id == id);
        }

        public IEnumerable<EventViewModel> GetUpcoming(EventType type, Guid clubId, bool showAll = false)
        {

            var queryable = _dbContext.Events
                .Where(t => t.ClubId == clubId)
                .Where(t => type == EventType.Alle || t.Type == type)
                .Where(t => t.DateTime >= DateTime.Now);

            if (!showAll)
                queryable = queryable.Where(t => t.SignupHasOpened() || (t.Type == EventType.Kamp && t.GameType == GameType.Treningskamp));


            var query = queryable
                .Include(t => t.EventTeams)
                .Include(t => t.Attendees)
                .ToList();

            var result = query
                .OrderBy(e => e.DateTime)
                .Select(e =>
                new EventViewModel(
                    e.ClubId, e.EventTeams.Select(et => et.TeamId).ToList(),
                    e.Attendees.Select(a => new AttendeeViewModel(a.MemberId, a.EventId, a.SignupMessage, a.IsAttending, a.DidAttend, a.IsSelected, null)).ToList(),
                    e.Id, e.Type, e.GameType, e.DateTime, e.Location, e.Headline, e.Description, e.Opponent, e.Voluntary, e.IsPublished, e.IsHomeTeam
                )).ToList();


            var memberIds = result.SelectMany(e => e.Attendees.Select(a => a.MemberId)).Distinct();

            var members =
                _dbContext.Members.Where(m => memberIds.Contains(m.Id))
                    .Select(m => new AttendeePlayerViewModel(m.Id, m.FirstName, m.LastName, m.UserName)).ToList();


            foreach (var attendee in result.SelectMany(ev => ev.Attendees))
            {
                attendee.Player = members.Single(m => m.Id == attendee.MemberId);
            }

            return result;
        }

 
        public IList<Event> GetAll(EventType type, Guid clubId)
        {
            return _dbContext.Events
                .Where(t => t.ClubId == clubId)
                .Where(t => type == EventType.Alle || t.Type == type)
                .ToList();
        }

        public IEnumerable<EventViewModel> GetPrevious(EventType type, Guid clubId)
        {
            var queryable = GetPastEvents(type, clubId);

            var result = queryable
                .Include(t => t.EventTeams)
                .Include(t => t.Attendees)
                .ToList();

            var resultViewModels = result.Select(e =>
                new EventViewModel(
                    e.ClubId, e.EventTeams.Select(et => et.TeamId),
                    e.Attendees.Select(a => new AttendeeViewModel(a.MemberId, a.EventId, a.SignupMessage, a.IsAttending, a.DidAttend, a.IsSelected, null)),
                    e.Id, e.Type, e.GameType, e.DateTime, e.Location, e.Headline, e.Description, e.Opponent, e.Voluntary, e.IsPublished, e.IsHomeTeam
                )).ToList();

            var memberIds = resultViewModels.SelectMany(e => e.Attendees.Select(a => a.MemberId)).Distinct();

            var members =
                _dbContext.Members.Where(m => memberIds.Contains(m.Id))
                    .Select(m => new AttendeePlayerViewModel(m.Id, m.FirstName, m.LastName, m.UserName)).ToList();


            foreach (var attendee in resultViewModels.SelectMany(ev => ev.Attendees))
            {
                attendee.Player = members.Single(m => m.Id == attendee.MemberId);
            }

            return resultViewModels;
        }

        private IOrderedQueryable<Event> GetPastEvents(EventType type, Guid clubId)
        {
            var result = _dbContext.Events
                .Where(t => t.ClubId == clubId)
                .Where(t => type == EventType.Alle || t.Type == type)
                .Where(t => t.DateTime < DateTime.Now)
                .OrderByDescending(e => e.DateTime);
            return result;
        }

        public AttendeeViewModel SetAttendance(Guid eventId, Guid memberId, bool isAttending, Guid clubId)
        {

            var attendance = _dbContext.EventAttendances.SingleOrDefault(e => e.EventId == eventId && e.MemberId == memberId);
            if (attendance != null)
            {
                attendance.IsAttending = isAttending;
            }
            else
            {
                attendance = new EventAttendance
                {
                    EventId = eventId,
                    MemberId = memberId,
                    IsAttending = isAttending,
                };
                _dbContext.EventAttendances.Add(attendance);
            }
            _dbContext.SaveChanges();
            _cacheHelper.ClearNotificationCacheByMemberId(clubId, memberId);

            return
                _dbContext.Players.Where(p => p.Id == memberId)
                    .Select(
                        p =>
                            new AttendeeViewModel(memberId, eventId, attendance.SignupMessage, isAttending, attendance.DidAttend, attendance.IsSelected,
                                new AttendeePlayerViewModel(memberId, p.FirstName, p.LastName, p.UserName)))
                    .Single();
        }

        public void Add(Guid clubId, params Event[] events)
        {
            _dbContext.Events.AddRange(events);
            _dbContext.SaveChanges();
            _cacheHelper.ClearNotificationCache(clubId);
        }

        public void Delete(Guid clubId, Guid eventId)
        {
            var ev = _dbContext.Events.Single(e => e.Id == eventId);
            _dbContext.Remove(ev);
            _dbContext.SaveChanges();
            _cacheHelper.ClearNotificationCache(clubId);
        }

        public void Update(CreateEventViewModel model, Guid clubId)
        {
            var ev = model.Type == EventType.Kamp ?
                 _dbContext.Games.Include(e => e.EventTeams)
                .Single(e => e.Id == model.EventId):
                _dbContext.Events.Include(e => e.EventTeams)
                .Single(e => e.Id == model.EventId);

            ev.Description = model.Description;
            ev.Headline = model.Headline;
            ev.Location = model.Location;
            ev.Opponent = model.Opponent;
            ev.DateTime = model.DateTime;
            ev.Voluntary = !model.Mandatory;
            ev.GameType = model.GameType;
            ev.IsHomeTeam = model.IsHomeTeam;

            if (ev.Type == EventType.Kamp)
            {
                ((Game)ev).TeamId = model.TeamIds.Single();
            }

            foreach (var id in model.TeamIds)
            {
                if (ev.EventTeams.All(e => e.TeamId != id))
                {
                    ev.EventTeams.Add(new EventTeam
                    {
                        Id = Guid.NewGuid(),
                        TeamId = id,
                        EventId = model.EventId.Value
                    });
                }
            }
            _dbContext.SaveChanges();
            foreach (var eventTeam in ev.EventTeams)
            {

                if (model.TeamIds.All(tid => eventTeam.TeamId != tid))
                {
                    _dbContext.EventTeams.Remove(eventTeam);
                }
            }
            _dbContext.SaveChanges();
            _cacheHelper.ClearNotificationCache(clubId);
        }

        public void ConfirmAttendance(Guid eventId, Guid playerId, bool didAttend)
        {
            var attendance = _dbContext.EventAttendances.FirstOrDefault(a => a.EventId == eventId && a.MemberId == playerId);
            if (attendance != null)
            {
                attendance.DidAttend = didAttend;
            }
            else
            {
                attendance = new EventAttendance
                {
                    Id = Guid.NewGuid(),
                    EventId = eventId,
                    DidAttend = didAttend,
                    IsAttending = false,
                    MemberId = playerId
                };
                _dbContext.EventAttendances.Add(attendance);
            }
            _dbContext.SaveChanges();
        }

        public EventViewModel GetEventViewModel(Guid eventId)
        {
            return _dbContext.Events.Where(e => e.Id == eventId).Select(e =>
                new EventViewModel(
                    e.ClubId, e.EventTeams.Select(et => et.TeamId).ToList(),
                    e.Attendees.Select(a => new AttendeeViewModel(a.MemberId, eventId, a.SignupMessage,  a.IsAttending, a.DidAttend, a.IsSelected, new AttendeePlayerViewModel(a.MemberId, a.Member.FirstName, a.Member.LastName, a.Member.UserName))).ToList(),
                    e.Id, e.Type, e.GameType, e.DateTime, e.Location, e.Headline, e.Description, e.Opponent, e.Voluntary, e.IsPublished, e.IsHomeTeam
                )).Single();
        }

        public RegisterAttendanceEventViewModel GetRegisterAttendanceEventViewModel(Guid? eventId)
        {
            var events = eventId != null
                ? _dbContext.Events.Where(e => e.Id == eventId)
                : _dbContext.Events.Where(e => e.Type == EventType.Trening && e.DateTime < DateTime.Now).OrderByDescending(e => e.DateTime);

            return events
                .Select(e => new RegisterAttendanceEventViewModel
                {
                    DateTime = e.DateTime,
                    Attendees = e.Attendees.Select(a => new RegisterAttendanceAttendeeViewModel
                    {
                        MemberId = a.MemberId,
                        IsAttending = a.IsAttending,
                        DidAttend = a.DidAttend,
                        WonTraining = a.WonTraining
                    }).ToList(),
                    Id = e.Id,
                    Location = e.Location,
                    Type = e.Type
                }).First();
        }

        public IEnumerable<SimpleEventViewModel> GetPreviousSimpleEvents(EventType trening, Guid clubId, int take)
        {
            var query = GetPastEvents(trening, clubId);

            return query.Take(15).Select(e => new SimpleEventViewModel
            {
                Id = e.Id,
                DateTime = e.DateTime

            }).ToList();
        }

        public void SignupMessage(Guid eventId, Guid memberId, string message)
        {
            var attendance = _dbContext.EventAttendances.Single(a => a.EventId == eventId && a.MemberId == memberId);

            attendance.SignupMessage = message;
            _dbContext.SaveChanges();
        }

        public void UpdateDescription(Guid eventId, string description)
        {
            var ev = _dbContext.Events.Single(e => e.Id == eventId);
            ev.Description = description;
            _dbContext.SaveChanges();
        }

        public void ConfirmTrainingVictory(Guid eventId, Guid playerId, bool didWin)
        {
            var attendance = _dbContext.EventAttendances.FirstOrDefault(a => a.EventId == eventId && a.MemberId == playerId);
            if (attendance != null)
            {
                attendance.WonTraining = didWin;
            }
            else
            {
                attendance = new EventAttendance
                {
                    Id = Guid.NewGuid(),
                    EventId = eventId,
                    WonTraining = didWin,
                    IsAttending = false,
                    MemberId = playerId
                };
                _dbContext.EventAttendances.Add(attendance);
            }
            _dbContext.SaveChanges();
        }
    }
}