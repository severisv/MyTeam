using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyTeam.Models.Enums
{
    public enum GameType
    {
        Treningskamp,
        Seriekamp,
        Norgesmesterskapet,
        Kretsmesterskapet,
        [Display(Name = "OBOS Cup")]
        ObosCup
    }
}

namespace MyTeam.Models.Domain
{



    public class Game : Event
    {
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public string GamePlan { get; set; }
        public string GamePlanState { get; set; }
        public virtual Article Report { get; set; }
        public virtual ICollection<GameEvent> GameEvents { get; set; }

    }
}