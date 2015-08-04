﻿using System.Collections.Generic;
using System.Linq;

namespace MyTeam.Models.Domain
{
    public class Team : Entity
    {
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public virtual Club Club { get; set; }
        public virtual IList<Event> Events { get; set; }
        public virtual IEnumerable<Event> Games => Events.Where(e => e.IsGame);
        public virtual IEnumerable<Event> Trainings => Events.Where(e => e.IsTraining);
    }
}