using System;
using MyTeam.Models.Shared;

namespace MyTeam.ViewModels.Events
{
    public class SimpleEventViewModel : IEvent
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
    }
}