using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
using MyTeam.Services.Repositories;

namespace MyTeam.Services.Domain
{
    public class EventService : IEventService
    {
        public IRepository<Event> EventRepository { get; set; }
        public IRepository<EventAttendance> EventAttendanceRepository { get; set; }

        public EventService(IRepository<Event> eventRepository, IRepository<EventAttendance> eventAttendanceRepository)
        {
            EventRepository = eventRepository;
            EventAttendanceRepository = eventAttendanceRepository;
        }

        public Event Get(Guid id)
        {
            return EventRepository.Get(id).Single();
        }

        public IEnumerable<Event> GetUpcoming(EventType type)
        {
            return EventRepository.Get()
                .Where(t => type == EventType.Alle || t.Type == type)
                .Where(t => t.DateTime.Date >= DateTime.Today.Date);
        }

        public IList<Event> GetAll(EventType type)
        {
            return EventRepository.Get()
                .Where(t => type == EventType.Alle || t.Type == type)
                .ToList();
        }

        public IEnumerable<Event> GetPrevious(EventType type)
        {
            return EventRepository.Get()
                .Where(t => type == EventType.Alle || t.Type == type)
                .Where(t => t.DateTime.Date < DateTime.Today.Date);
        }

        public Event SetAttendanceReturnsEvent(Guid playerId, Guid eventId, bool isAttending)
        {
            var ev = EventRepository.Get(eventId).Single();

            var attendance = ev.Attendees.SingleOrDefault(a => a.PlayerId == playerId);
            if (attendance != null)
            {
                attendance.IsAttending = isAttending;
                EventAttendanceRepository.Update(attendance);
            }
            else
            {
                attendance = new EventAttendance
                {
                    EventId = eventId,
                    PlayerId = playerId,
                    IsAttending = isAttending
                };
                EventAttendanceRepository.Add(attendance);

            }

            ev.Attendees.Add(attendance);
            return ev;

        }
    }
}