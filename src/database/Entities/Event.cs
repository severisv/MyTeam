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
    public class Event : Entity, IEvent
    {
        [Required]
        public Guid ClubId { get; set; }
        [Required]
        public EventType Type { get; set; }
        public GameType? GameType { get; set; }

        [Required]
        public DateTime DateTime { get; set; }
        [Required]
        public string Location { get; set; }
        public string Headline { get; set; }
        public string Description { get; set; }

        public string Opponent { get; set; }
        public bool Voluntary { get; set; }

        public bool? GamePlanIsPublished { get; set; }

        public virtual Club Club { get; set; }
        
        public virtual ICollection<EventTeam> EventTeams { get; set; }
        public virtual ICollection<EventAttendance> Attendees { get; set; }
        [NotMapped]
        public virtual IEnumerable<Member> Attending => Attendees?.Where(a => a.IsAttending == true).Select(a => a.Member);
        [NotMapped]
        public virtual IEnumerable<Member> NotAttending => Attendees?.Where(a => a.IsAttending == false).Select(a => a.Member);

        [NotMapped]
        public bool IsGame => Type == EventType.Kamp;
        [NotMapped]
        public bool IsTraining => Type == EventType.Trening;
        [NotMapped]
        public bool IsCustom => Type == EventType.Diverse;

        public bool IsPublished { get; set; }
        public bool IsHomeTeam { get; set; }

        public bool IsAttending(ClaimsPrincipal user) => Attending?.Any(a => a.UserName == user.Identity.Name) == true;
   
        public bool IsNotAttending(ClaimsPrincipal user) => NotAttending?.Any(a => a.UserName == user.Identity.Name) == true;

        public bool SignupHasOpened()
        {
            if (Type == EventType.Diverse) return true;
            return DateTime.Date - DateTime.Now.Date < new TimeSpan(Settings.Config.AllowedSignupDays, 0, 0, 0, 0);
        }
    }
}