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
        IEnumerable<Event> GetUpcoming(EventType type, bool showAll = false);
        IList<Event> GetAll(EventType type);
        IEnumerable<Event> GetPrevious(EventType type, int? count = null);
        void SetAttendance(Event ev, Guid playerId,  bool isAttending);
        void Add(params  Event[] ev);
        void Delete(Guid eventId);
        void Update(Event ev);
        void ConfirmAttendance(Guid attendanceId, bool didAttend);
    }
}
