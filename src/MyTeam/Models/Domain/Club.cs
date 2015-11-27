using System.Collections.Generic;

namespace MyTeam.Models.Domain
{
    public class Club : Entity
    {
        public string ClubId { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public string Favicon { get; set; }
        public virtual List<Team> Teams { get; set; }

        public virtual List<Player> Players { get; set; } 
    }
}