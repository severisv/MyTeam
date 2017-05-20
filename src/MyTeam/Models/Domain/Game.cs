using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyTeam.Models.Domain
{
    public class Game : Event
    {
        [Required]
        public Guid TeamId { get; set; }
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public string GamePlan { get; set; }
        public virtual Team Team { get; set; }
        public virtual Article Report { get; set; }
        public virtual ICollection<GameEvent> GameEvents { get; set; }

    }
}