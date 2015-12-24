using System;
using System.Collections.Generic;
using System.Linq;
using MyTeam.Models;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
using MyTeam.Services.Repositories;
using MyTeam.ViewModels.Events;
using Microsoft.Data.Entity;
using MyTeam.Services.Application;

namespace MyTeam.Services.Domain
{
    public class EventService : IEventService
    {
        public IRepository<Event> EventRepository { get; set; }
        public IRepository<EventAttendance> EventAttendanceRepository { get; set; }
        public IRepository<Player> PlayerRepository { get; set; }
        private readonly ApplicationDbContext _dbContext;
        private readonly ICacheHelper _cacheHelper;

        public EventService(IRepository<Event> eventRepository, IRepository<EventAttendance> eventAttendanceRepository, IRepository<Player> playerRepository, ApplicationDbContext dbContext, ICacheHelper cacheHelper)
        {
            EventRepository = eventRepository;
            EventAttendanceRepository = eventAttendanceRepository;
            PlayerRepository = playerRepository;
            _dbContext = dbContext;
            _cacheHelper = cacheHelper;
        }

        public Event Get(Guid id)
        {
            return EventRepository.GetSingle(id);
        }

        public IEnumerable<EventViewModel> GetUpcoming(EventType type, IEnumerable<Guid> teamIds, bool showAll = false)
        {
            return EventRepository.Get()
                .Where(t => teamIds.Any(ti => t.EventTeams.Any(et => et.TeamId == ti)))  
                .Where(t => type == EventType.Alle || t.Type == type)
                .Where(t => t.DateTime >= DateTime.Now)
                .Where(t => t.SignupHasOpened())
                .OrderBy(e => e.DateTime)
                .Select(e =>
                new EventViewModel(
                    e.ClubId, e.EventTeams.Select(et => et.TeamId),
                    e.Attendees.Select(a => new AttendeeViewModel(a.MemberId, a.EventId, a.Member.FirstName, a.Member.LastName, a.Member.UserName, a.IsAttending, a.DidAttend)),
                    e.Id, e.Type, e.DateTime, e.Location, e.Headline, e.Description, e.Opponent, e.Voluntary
                )).ToList();
        }

 
        public IList<Event> GetAll(EventType type, IEnumerable<Guid> teamIds)
        {
            return EventRepository.Get()
                .Where(t => teamIds.Any(ti => t.EventTeams.Any(et => et.TeamId == ti)))
                .Where(t => type == EventType.Alle || t.Type == type)
                .ToList();
        }

        public IEnumerable<EventViewModel> GetPrevious(EventType type, IEnumerable<Guid> teamIds, int? count = null)
        {
            var result = EventRepository.Get()
                .Where(t => teamIds.Any(ti => t.EventTeams.Any(et => et.TeamId == ti)))
                .Where(t => type == EventType.Alle || t.Type == type)
                .Where(t => t.DateTime < DateTime.Now)
                .OrderByDescending(e => e.DateTime);

            var resultViewModels = result.Select(e =>
                new EventViewModel(
                    e.ClubId, e.EventTeams.Select(et => et.TeamId),
                    e.Attendees.Select(a => new AttendeeViewModel(a.MemberId, a.EventId, a.Member.FirstName, a.Member.LastName, a.Member.UserName, a.IsAttending, a.DidAttend)),
                    e.Id, e.Type, e.DateTime, e.Location, e.Headline, e.Description, e.Opponent, e.Voluntary
                )).ToList();

            if (count != null)
            {
                return resultViewModels.Take(count.Value);
            }
            return resultViewModels;
        }

        public void SetAttendance(Guid eventId, Guid memberId, bool isAttending, Guid clubId)
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

        }

        public void Add(Guid clubId, params Event[] events)
        {
            _dbContext.Events.AddRange(events);
            _dbContext.SaveChanges();
            _cacheHelper.ClearNotificationCache(clubId);
        }

        public void Delete(Guid clubId, Guid eventId)
        {
            var ev = EventRepository.GetSingle(eventId);
            _dbContext.Remove(ev);
            _dbContext.SaveChanges();
            _cacheHelper.ClearNotificationCache(clubId);
        }

        public void Update(CreateEventViewModel model, Guid clubId)
        {
            var ev = _dbContext.Events.Include(e => e.EventTeams)
                .Single(e => e.Id == model.EventId);
            ev.Description = model.Description;
            ev.Headline = model.Headline;
            ev.Location = model.Location;
            ev.Opponent = model.Opponent;
            ev.DateTime = model.DateTime;
            ev.Voluntary = !model.Mandatory;

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
            var attendance = EventAttendanceRepository.Get().FirstOrDefault(a => a.EventId == eventId && a.MemberId == playerId);
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
                    e.ClubId, e.EventTeams.Select(et => et.TeamId),
                    e.Attendees.Select(a => new AttendeeViewModel(a.MemberId, eventId, a.Member.FirstName, a.Member.LastName, a.Member.UserName, a.IsAttending, a.DidAttend)),
                    e.Id, e.Type, e.DateTime, e.Location, e.Headline, e.Description, e.Opponent, e.Voluntary
                )).Single();
        }
    }
}