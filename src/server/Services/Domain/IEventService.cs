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
        PagedList<EventViewModel> GetUpcoming(EventType type, Guid clubId, IEnumerable<Guid> teamIds, bool showAll = false);
        PagedList<EventViewModel> GetPrevious(IEnumerable<Guid> teamIds, EventType type, int page);
        AttendeeViewModel SetAttendance(Guid eventId, Guid playerId, bool isAttending, Guid clubId);
        void Add(Guid clubId, params Event[] ev);
        void Delete(Guid clubId, Guid eventId);
        void Update(CreateEventViewModel ev, Guid clubId);
        void ConfirmAttendance(Guid eventId, Guid playerId, bool didAttend);
        EventViewModel GetEventViewModel(Guid eventId);
        RegisterAttendanceEventViewModel GetRegisterAttendanceEventViewModel(Guid? eventId);
        IEnumerable<SimpleEventViewModel> GetPreviousSimpleEvents(EventType trening, Guid id, int i);
        void SignupMessage(Guid eventId, Guid memberId, string message);
        SignupDetailsViewModel GetSignupDetailsViewModel(Guid eventId);
    }
}
