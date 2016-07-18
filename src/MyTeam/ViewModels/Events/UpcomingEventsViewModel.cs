using System.Collections.Generic;
using System.Linq;
using MyTeam.Models.Enums;
using MyTeam.Resources;

namespace MyTeam.ViewModels.Events
{
    public class UpcomingEventsViewModel
    {

        public IEnumerable<EventViewModel> Events { get; }
        public EventType Type { get; }
        public bool Previous { get; }
        public string Title => $"{(Previous ? Res.Previous : Res.Upcoming)} {(Type == EventType.Alle ? Res.Event.ToLower() : Type.ToString().ToLower()).Pluralize()}";

        readonly UserMember _user;

        public UpcomingEventsViewModel(IEnumerable<EventViewModel> events, EventType type, UserMember user, bool previous)
        {
            Type = type;
            Previous = previous;
            _user = user;

            Events = !_user.Roles.Contains(Roles.Admin) ? 
                events.Where(e => e.TeamIds.ContainsAny(_user.TeamIds)) : 
                events;

        }
    }
}