using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using MyTeam.Models;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
using MyTeam.Services.Repositories;
using MyTeam.ViewModels.Events;

namespace MyTeam.Services.Domain
{
    public class EventService : IEventService
    {
        public IRepository<Event> EventRepository { get; set; }
        public IRepository<EventAttendance> EventAttendanceRepository { get; set; }
        public IRepository<Player> PlayerRepository { get; set; }
        private readonly ApplicationDbContext _dbContext;

        public EventService(IRepository<Event> eventRepository, IRepository<EventAttendance> eventAttendanceRepository, IRepository<Player> playerRepository, ApplicationDbContext dbContext)
        {
            EventRepository = eventRepository;
            EventAttendanceRepository = eventAttendanceRepository;
            PlayerRepository = playerRepository;
            _dbContext = dbContext;
        }

        public Event Get(Guid id)
        {
            return EventRepository.GetSingle(id);
        }

        public IEnumerable<EventViewModel> GetUpcoming(EventType type, Guid clubId, bool showAll = false)
        {
            return EventRepository.Get()
                .Where(t => t.ClubId == clubId)
                .Where(t => type == EventType.Alle || t.Type == type)
                .Where(t => t.DateTime >= DateTime.Now)
                .Where(t => t.SignupHasOpened())
                .OrderBy(e => e.DateTime)
                .Select(e =>
                new EventViewModel(
                    e.Attendees.Select(a => new AttendeeViewModel(a.MemberId, a.EventId, a.Member.FirstName, a.Member.LastName, a.Member.UserName, a.IsAttending, a.DidAttend)),
                     e.Id, type, e.DateTime, e.Location, e.Headline, e.Description, e.Opponent, e.Voluntary
                ));
        }

        public IList<Event> GetAll(EventType type, Guid clubId)
        {
            return EventRepository.Get()
                .Where(t => t.ClubId == clubId)
                .Where(t => type == EventType.Alle || t.Type == type)
                .ToList();
        }

        public IEnumerable<EventViewModel> GetPrevious(EventType type, Guid clubId, int? count = null)
        {
            var result = EventRepository.Get()
                .Where(t => t.ClubId == clubId)
                .Where(t => type == EventType.Alle || t.Type == type)
                .Where(t => t.DateTime < DateTime.Now)
                .OrderByDescending(e => e.DateTime);

            var resultViewModels = result.Select(e =>
                new EventViewModel(
                    e.Attendees.Select(a => new AttendeeViewModel(a.MemberId, a.EventId, a.Member.FirstName, a.Member.LastName, a.Member.UserName, a.IsAttending, a.DidAttend)),
                    e.Id, type, e.DateTime, e.Location, e.Headline, e.Description, e.Opponent, e.Voluntary
                ));

            if (count != null)
            {
                return resultViewModels.Take(count.Value);
            }
            return resultViewModels;
        }

        public void SetAttendance(Guid eventId, Guid memberId, bool isAttending)
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

        }

        public void Add(params Event[] events)
        {
            _dbContext.Events.AddRange(events);
            _dbContext.SaveChanges();
        }

        public void Delete(Guid eventId)
        {
            var ev = EventRepository.GetSingle(eventId);
            _dbContext.Remove(ev);
            _dbContext.SaveChanges();
        }

        public void Update(CreateEventViewModel model)
        {
            var ev = _dbContext.Events.Single(e => e.Id == model.EventId);
            ev.Description = model.Description;
            ev.Headline = model.Headline;
            ev.Location = model.Location;
            ev.Opponent = model.Opponent;
            ev.DateTime = model.DateTime;
            ev.Voluntary = !model.Mandatory;
            _dbContext.SaveChanges();
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
                    e.Attendees.Select(a => new AttendeeViewModel(a.MemberId, eventId, a.Member.FirstName, a.Member.LastName, a.Member.UserName, a.IsAttending, a.DidAttend)),
                    e.Id, e.Type, e.DateTime, e.Location, e.Headline, e.Description, e.Opponent, e.Voluntary
                )).Single();
        }
    }
}