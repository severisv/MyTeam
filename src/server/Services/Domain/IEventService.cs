using System;
using System.Collections.Generic;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
using MyTeam.Models.General;
using MyTeam.ViewModels.Events;

namespace MyTeam.Services.Domain
{
    public interface IEventService
    {
        Event Get(Guid id);
        void Add(Guid clubId, params Event[] ev);
        void Delete(Guid clubId, Guid eventId);
        void Update(CreateEventViewModel ev, Guid clubId);
        EventViewModel GetEventViewModel(Guid eventId);
    }
}
