using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using MyTeam.Models.Domain;
using MyTeam.Services.Repositories;

namespace MyTeam.Services.Domain
{
    public class EventService<TType> : IEventService<TType> where TType : Event
    {
        public IRepository<TType> EventRepository { get; set; }

        public EventService(IRepository<TType> eventRepository)
        {
            EventRepository = eventRepository;
        }

        public TType Get(Guid id)
        {
            return EventRepository.Get(id).Single();
        }

        public IEnumerable<TType> GetUpcoming()
        {
            return EventRepository.Get().Where(t => t.Date >= DateTime.Today.Date);
        }

        public IList<TType> GetAll()
        {
            return EventRepository.Get().ToList();
        }

        public IEnumerable<TType> GetPrevious()
        {
            return EventRepository.Get().Where(t => t.Date < DateTime.Today.Date);
        }
    }
}