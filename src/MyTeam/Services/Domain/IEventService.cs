using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
using MyTeam.ViewModels.Events;

namespace MyTeam.Services.Domain
{
    public interface IEventService
    {
        Event Get(Guid id);
        IEnumerable<EventViewModel> GetUpcoming(EventType type, Guid clubId, bool showAll = false);
        IList<Event> GetAll(EventType type, Guid clubId);
        IEnumerable<EventViewModel> GetPrevious(EventType type, Guid clubId, int? count = null);
        void SetAttendance(Guid eventId, Guid playerId,  bool isAttending);
        void Add(params  Event[] ev);
        void Delete(Guid eventId);
        void Update(CreateEventViewModel ev);
        void ConfirmAttendance(Guid eventId, Guid playerId, bool didAttend);
        EventViewModel GetEventViewModel(Guid eventId);
    }
}
