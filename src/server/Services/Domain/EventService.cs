using System;
using System.Collections.Generic;
using System.Linq;
using MyTeam.Models;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
using MyTeam.ViewModels.Events;
using Microsoft.EntityFrameworkCore;
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
   


        public void Add(Guid clubId, params Event[] events)
        {
            foreach (var @event in events)
            {
                @event.ClubId = clubId;
            }
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
                .Single(e => e.Id == model.EventId) :
                _dbContext.Events.Include(e => e.EventTeams)
                .Single(e => e.Id == model.EventId);

            ev.Description = model.Description;
            ev.Headline = model.Headline;
            ev.Location = model.Location;
            ev.Opponent = model.Opponent;
            ev.DateTime = model.DateTime;
            ev.Voluntary = !model.Mandatory;
            ev.GameTypeValue = model.GameType;
            ev.IsHomeTeam = model.IsHomeTeam;

            if (ev.Type.FromInt() == EventType.Kamp)
            {
                (ev).TeamId = model.TeamIds.Single();
            }

            else
            {
                foreach (var id in model.TeamIds)
                {
                    if (ev.EventTeams.All(e => e.TeamId != id))
                    {
                        _dbContext.EventTeams.Add(new EventTeam
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
            }
           
            
            _dbContext.SaveChanges();
            _cacheHelper.ClearNotificationCache(clubId);
        }

        public EventViewModel GetEventViewModel(Guid eventId) =>
             _dbContext.Events.Where(e => e.Id == eventId).Select(e =>
                new EventViewModel(
                    e.ClubId, e.EventTeams.Select(et => et.TeamId).ToList(),
                    e.Id, e.Type.FromInt(), e.GameTypeValue, e.DateTime, e.Location, e.Headline, e.Description, e.Opponent, e.Voluntary, e.IsPublished, e.IsHomeTeam, e.GamePlanIsPublished
                        )).First();

    }
}