using System;
using System.Collections.Generic;
using MyTeam.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using MyTeam.Models.Shared;

namespace MyTeam.Models.Domain
{



    public class Event
    {
        public Guid Id { get; set; }

        public Guid ClubId { get; set; }
        
        public Guid? TeamId { get; set; }
        public virtual Team Team { get; set; }
        public int Type { get; set; }
        public int? GameType { get; set; }
        [NotMapped]
        [Obsolete]
        public virtual GameType? GameTypeValue
        {
            get { return (GameType?)GameType; }
            set { GameType = (int?)value; }
        }

        [Required]
        public DateTime DateTime { get; set; }
        [Required]
        public string Location { get; set; }
        public string Headline { get; set; }
        public string  Description { get; set; }

        public string Opponent { get; set; }
        public bool Voluntary { get; set; }

        public bool? GamePlanIsPublished { get; set; }

        public virtual Club Club { get; set; }

        public virtual ICollection<EventTeam> EventTeams { get; set; }
        public virtual ICollection<EventAttendance> Attendees { get; set; }

        public bool IsPublished { get; set; }
        public bool IsHomeTeam { get; set; }
        
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public string GamePlan { get; set; }
        public string GamePlanState { get; set; }
        public virtual Article Report { get; set; }
        public virtual ICollection<GameEvent> GameEvents { get; set; }
        
    }
}