using MyTeam.Models.Enums;
using MyTeam.Models.General;
using MyTeam.Resources;

namespace MyTeam.ViewModels.Events
{
    public class UpcomingEventsViewModel
    {

        public PagedList<EventViewModel> Events { get; }
        public EventType Type { get; }
        public bool Previous { get; }
        public string Title => $"{(Previous ? Res.Previous : Res.Upcoming)} {(Type == EventType.Alle ? Res.Event.ToLower() : Type.ToString().ToLower()).Pluralize()}";

        public UpcomingEventsViewModel(PagedList<EventViewModel> events, EventType type,  bool previous)
        {
            Type = type;
            Previous = previous;
            Events = events;
        }
    }
}