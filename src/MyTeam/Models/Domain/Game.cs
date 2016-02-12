using System;

namespace MyTeam.Models.Domain
{
    public class Game : Event
    {

        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public virtual Article Report { get; set; }

    }
}