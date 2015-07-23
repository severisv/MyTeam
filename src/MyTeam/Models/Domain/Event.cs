using System;
using System.Collections;
using System.Collections.Generic;

namespace MyTeam.Models.Domain
{
    public abstract class Event : Entity
    {
        public DateTime DateTime { get; set; }
        public DateTime Date => DateTime.Date;
        public TimeSpan Time => DateTime.TimeOfDay;
        public string Location { get; set; }
        public string Description { get; set; }

        public virtual IEnumerable<Player> Attendees { get; set; }
    }
}