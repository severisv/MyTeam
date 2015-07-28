using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;

namespace MyTeam.Services.Domain
{
    public interface IEventService
    {
        Event Get(Guid id);
        IEnumerable<Event> GetUpcoming(EventType type);
        IList<Event> GetAll(EventType type);
        IEnumerable<Event> GetPrevious(EventType type);
        Event SetAttendanceReturnsEvent(Guid id, Guid eventId, bool isAttending);
    }
}
