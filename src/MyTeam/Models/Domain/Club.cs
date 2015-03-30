using System.Collections.Generic;

namespace MyTeam.Models.Domain
{
    public class Club : Entity
    {
        public string Name { get; set; }
        public virtual List<Team> Teams { get; set; }

        public virtual List<Player> Players { get; set; } 
    }
}