using System;
using System.Collections;
using System.Collections.Generic;
using MyTeam.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;

namespace MyTeam.Models.Domain
{
    public class Event : Entity
    {

        public EventType Type { get; set; } 

        public DateTime DateTime { get; set; }
        public string Location { get; set; }
        public string Headline { get; set; }
        public string Description { get; set; }
        public string Opponent { get; set; }
        public bool Voluntary { get; set; }

        public virtual IList<EventAttendance> Attendees { get; set; }
        public virtual IEnumerable<Player> Attending => Attendees.Where(a => a.IsAttending).Select(a => a.Player);
        public virtual IEnumerable<Player> NotAttending => Attendees.Where(a => !a.IsAttending).Select(a => a.Player);

        public bool IsGame => Type == EventType.Kamp;
        public bool IsTraining => Type == EventType.Trening;
        public bool IsCustom => Type == EventType.Diverse;

        public bool IsAttending(ClaimsPrincipal user)
        {
            return Attending.Any(a => a.UserName == user.Identity.Name);
        }
        public bool IsNotAttending(ClaimsPrincipal user)
        {
            return NotAttending.Any(a => a.UserName == user.Identity.Name);
        }

        public bool SignupHasOpened()
        {
            if (Type == EventType.Diverse) return true;
            return DateTime.Date - DateTime.Now.Date < new TimeSpan(Settings.Config.AllowedSignupDays,0,0,0,0);
        }

        public bool SignupHasClosed()
        {
            return DateTime < DateTime.Now;
        }

        public bool SignoffHasClosed()
        {
            return DateTime - DateTime.Now < new TimeSpan(0,Settings.Config.AllowedSignoffHours,0,0);
        }
    }
}