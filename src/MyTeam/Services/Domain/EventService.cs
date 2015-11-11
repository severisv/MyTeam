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

        public IEnumerable<Event> GetUpcoming(EventType type, bool showAll = false)
        {
            return EventRepository.Get()
                .Where(t => type == EventType.Alle || t.Type == type)
                .Where(t => t.DateTime.Date >= DateTime.Today.Date)
                .Where(t => t.SignupHasOpened())
                .OrderBy(e => e.DateTime);
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
                .Where(t => t.DateTime.Date < DateTime.Today.Date)
                .OrderByDescending(e => e.DateTime);
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

        public void Add(params Event[] events)
        {
            foreach (var ev in events)
            {
                ev.Attendees = new List<EventAttendance>();
            }
            EventRepository.Add(events);
        }

        public void Delete(Guid eventId)
        {
            var ev = EventRepository.Get(eventId).Single();
            EventRepository.Delete(ev);
        }

        public void Update(Event ev)
        {
            EventRepository.Update(ev);
        }

        public void ConfirmAttendance(Guid attendanceId, bool didAttend)
        {
            var attendance = EventAttendanceRepository.GetSingle(attendanceId);
            attendance.DidAttend = didAttend;
            EventAttendanceRepository.CommitChanges();
        }
    }
}