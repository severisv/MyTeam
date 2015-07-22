using System.Collections.Generic;
using System.Linq;

namespace MyTeam.Models.Domain
{
    public class Team
    {
        public string Name { get; set; }
        public virtual Club Club { get; set; }
        public virtual IList<Event> Events { get; set; }
        public virtual IEnumerable<Game> Games => Events.OfType<Game>();
        public virtual IEnumerable<Training> Trainings => Events.OfType<Training>();
    }
}