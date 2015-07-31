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
        public IRepository<Player> PlayerRepository { get; set; }

        public EventService(IRepository<Event> eventRepository, IRepository<EventAttendance> eventAttendanceRepository, IRepository<Player> playerRepository)
        {
            EventRepository = eventRepository;
            EventAttendanceRepository = eventAttendanceRepository;
            PlayerRepository = playerRepository;
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

        public void SetAttendance(Event ev, Guid playerId, bool isAttending)
        {

            var attendance = ev.Attendees.SingleOrDefault(a => a.PlayerId == playerId);
            if (attendance != null)
            {
                attendance.IsAttending = isAttending;
            }
            else
            {
                attendance = new EventAttendance
                {
                    EventId = ev.Id,
                    PlayerId = playerId,
                    IsAttending = isAttending,
                    Player = PlayerRepository.GetSingle(playerId),
                    Event = ev
                };
                EventAttendanceRepository.Add(attendance);
                ev.Attendees.Add(attendance);
            }

        }

        public void Add(Event ev)
        {
            EventRepository.Add(ev);
        }

        public void Delete(Guid eventId)
        {
            var ev = EventRepository.Get(eventId).Single();
            EventRepository.Delete(ev);
        }
    }
}