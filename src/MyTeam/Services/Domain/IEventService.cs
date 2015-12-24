using System;
using System.Collections.Generic;
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
        void SetAttendance(Guid eventId, Guid playerId,  bool isAttending, Guid clubId);
        void Add(Guid clubId, params  Event[] ev);
        void Delete(Guid clubId, Guid eventId);
        void Update(CreateEventViewModel ev, Guid clubId);
        void ConfirmAttendance(Guid eventId, Guid playerId, bool didAttend);
        EventViewModel GetEventViewModel(Guid eventId);
        RegisterAttendanceEventViewModel GetRegisterAttendanceEventViewModel(Guid eventId);
        IEnumerable<SimpleEventViewModel> GetPreviousSimpleEvents(EventType trening, Guid id, int i);
    }
}
