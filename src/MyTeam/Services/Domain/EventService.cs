using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using MyTeam.Models;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
using MyTeam.Services.Repositories;

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

        public IEnumerable<Event> GetUpcoming(EventType type, Guid clubId, bool showAll = false)
        {
            return EventRepository.Get()
                .Where(t => t.ClubId == clubId)
                .Where(t => type == EventType.Alle || t.Type == type)
                .Where(t => t.DateTime >= DateTime.Now)
                .Where(t => t.SignupHasOpened())
                .OrderBy(e => e.DateTime);
        }

        public IList<Event> GetAll(EventType type, Guid clubId)
        {
            return EventRepository.Get()
                .Where(t => t.ClubId == clubId)
                .Where(t => type == EventType.Alle || t.Type == type)
                .ToList();
        }

        public IEnumerable<Event> GetPrevious(EventType type, Guid clubId, int? count = null)
        {
            var result = EventRepository.Get()
                .Where(t => t.ClubId == clubId)
                .Where(t => type == EventType.Alle || t.Type == type)
                .Where(t => t.DateTime < DateTime.Now)
                .OrderByDescending(e => e.DateTime);

            if (count != null) return result.Take((int)count);
            return result;
        }

        public void SetAttendance(Event ev, Guid playerId, bool isAttending)
        {

            var attendance = ev.Attendees?.SingleOrDefault(a => a.MemberId == playerId);
            if (attendance != null)
            {
                attendance.IsAttending = isAttending;
            }
            else
            {
                attendance = new EventAttendance
                {
                    EventId = ev.Id,
                    MemberId = playerId,
                    IsAttending = isAttending,
                };
                _dbContext.EventAttendances.Add(attendance);
                _dbContext.SaveChanges();
            }

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

        public void Update(Event ev)
        {
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
    }
}