using System;
using System.Collections;
using System.Collections.Generic;
using MyTeam.Models.Enums;

namespace MyTeam.Models.Domain
{
    public class Event : Entity
    {
        public EventType Type { get; set; } 

        public DateTime DateTime { get; set; }
        public DateTime Date => DateTime.Date;
        public TimeSpan Time => DateTime.TimeOfDay;
        public string Location { get; set; }
        public string Description { get; set; }

        public bool Recurring { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public virtual IEnumerable<Player> Attendees { get; set; }

        public bool IsGame => Type == EventType.Kamp;
        public bool IsTraining => Type == EventType.Trening;
        public bool IsCustom => Type == EventType.Annen;

    }
}