using System;
using System.Collections.Generic;
using MyTeam.Models.Enums;

namespace MyTeam.ViewModels.Game
{
    public class RegisterSquadEventViewModel
    {
        public IEnumerable<RegisterSquadAttendeeViewModel> Attendees { get; set; }
        public Guid Id { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public bool IsPublished { get; set; }
        public DateTime DateTime { get; set; }
        public EventType Type { get; set; }
        public IEnumerable<Guid> TeamIds { get; set; }
    }
}