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

        public EventService(IRepository<Event> eventRepository)
        {
            EventRepository = eventRepository;
        }

        public Event Get(Guid id)
        {
            return EventRepository.Get(id).Single();
        }

        public IEnumerable<Event> GetUpcoming(EventType type)
        {
            return EventRepository.Get()
                .Where(t => type == EventType.Alle || t.Type == type)
                .Where(t => t.Date >= DateTime.Today.Date);
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
                .Where(t => t.Date < DateTime.Today.Date);
        }
    }
}