using System.Collections.Generic;
using Microsoft.Data.Entity.Metadata;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
using MyTeam.Resources;

namespace MyTeam.ViewModels.Training
{
    public class UpcomingEventsViewModel
    {

        public IEnumerable<Event> Events { get; }
        public EventType Type { get; }
        public string Title => $"{Res.Upcoming} {(Type == EventType.Alle ? Res.Events.ToLower() : Type.ToString().ToLower())}";

        public UpcomingEventsViewModel(IEnumerable<Event> events, EventType type)
        {
            Type = type;
            Events = events;
        }
    }
}