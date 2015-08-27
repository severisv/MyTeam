using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Entity.Metadata;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
using MyTeam.Resources;

namespace MyTeam.ViewModels.Events
{
    public class UpcomingEventsViewModel
    {

        public IEnumerable<Event> Events { get; }
        public EventType Type { get; }
        public bool Previous { get; }
        public string Title => $"{(Previous ? Res.Previous : Res.Upcoming)} {(Type == EventType.Alle ? Res.Event.ToLower() : Type.ToString().ToLower()).Pluralize()}";

        public UpcomingEventsViewModel(IEnumerable<Event> events, EventType type, bool previous)
        {
            Type = type;
            Events = events.OrderBy(e => e.DateTime);
            Previous = previous;
        }
    }
}