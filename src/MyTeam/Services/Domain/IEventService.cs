using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;

namespace MyTeam.Services.Domain
{
    public interface IEventService
    {
        Event Get(Guid id);
        IEnumerable<Event> GetUpcoming(EventType type, Guid clubId, bool showAll = false);
        IList<Event> GetAll(EventType type, Guid clubId);
        IEnumerable<Event> GetPrevious(EventType type, Guid clubId, int? count = null);
        void SetAttendance(Event ev, Guid playerId,  bool isAttending);
        void Add(params  Event[] ev);
        void Delete(Guid eventId);
        void Update(Event ev);
        void ConfirmAttendance(Guid eventId, Guid playerId, bool didAttend);
    }
}
