using System;
using System.Collections;
using System.Collections.Generic;
using MyTeam.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace MyTeam.Models.Domain
{
    public class Event : Entity
    {
        public EventType Type { get; set; } 

        public DateTime DateTime { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }

        public bool Recurring { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public virtual IEnumerable<Player> Attendees { get; set; }
        public virtual IEnumerable<Player> Attending => Attendees;
        public virtual IEnumerable<Player> NotAttending => Attendees;

        public bool IsGame => Type == EventType.Kamp;
        public bool IsTraining => Type == EventType.Trening;
        public bool IsCustom => Type == EventType.Diverse;

    }
}