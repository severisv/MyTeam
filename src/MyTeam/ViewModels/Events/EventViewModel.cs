using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
using MyTeam.ViewModels.Table;

namespace MyTeam.ViewModels.Events
{
    public class EventViewModel
    {
        public Guid Id { get; }
        public Guid ClubId { get; }
        public EventType Type { get; }
        public DateTime DateTime { get;  }
        public string Location { get; }
        public string Headline { get;  }
        public string Description { get;  }
        public bool Voluntary { get; }
        public string Opponent { get; }
        public IEnumerable<Guid> TeamIds { get; }

        public IEnumerable<AttendeeViewModel> Attendees { get;  }

        public EventViewModel(Guid clubId, IEnumerable<Guid> teamIds, IEnumerable<AttendeeViewModel> attendees, Guid eventId, EventType type, DateTime dateTime, string location,
            string headline, string description, string opponent, bool voluntary)
        {
            Id = eventId;
            ClubId = clubId;
            Attendees = attendees;
            DateTime = dateTime;
            Location = location;
            Headline = headline;
            Description = description;
            Type = type;
            Opponent = opponent;
            Voluntary = voluntary;
            TeamIds = teamIds;
        }

        public EventViewModel(Event e) : this(e.ClubId, e.EventTeams.Select(t => t.TeamId), null, e.Id, e.Type, e.DateTime, e.Location, e.Headline, e.Description, e.Opponent, e.Voluntary)
        {
            
        }

     


        public IEnumerable<AttendeeViewModel> Attending => Attendees?.Where(a => a.IsAttending);
        public IEnumerable<AttendeeViewModel> NotAttending => Attendees?.Where(a => !a.IsAttending);
        public IEnumerable<AttendeeViewModel> DidAttend => Attendees?.Where(a => a.DidAttend);

        public bool IsGame => Type == EventType.Kamp;
        public bool IsTraining => Type == EventType.Trening;
        public bool IsCustom => Type == EventType.Diverse;

        public bool IsAttending(ClaimsPrincipal user)
        {
            return Attending?.Any(a => a.UserName == user.Identity.Name) == true;
        }
        public bool IsNotAttending(ClaimsPrincipal user)
        {
            return NotAttending?.Any(a => a.UserName == user.Identity.Name) == true;
        }

        public string GetSignupMessage(Guid memberId)
        {
            return Attendees?.Single(a => a.MemberId == memberId).SignupMessage;
        }

        public bool SignupHasOpened()
        {
            if (Type == EventType.Diverse) return true;
            return DateTime.Date - DateTime.Now.Date < new TimeSpan(Settings.Config.AllowedSignupDays, 0, 0, 0, 0);
        }

        public bool SignupHasClosed() => DateTime < DateTime.Now;

        public bool SignoffHasClosed() => DateTime - DateTime.Now < new TimeSpan(0, Settings.Config.AllowedSignoffHours, 0, 0);

        public bool HasPassed() => DateTime.Now - DateTime  > new TimeSpan(0, 1, 0, 0);

        public void SetAttendance(Guid memberId, bool isAttending)
        {
            foreach (var attendee in Attendees.Where(a => a.MemberId == memberId))
            {
                attendee.IsAttending = isAttending;
            }
        }

        public CurrentTeam Team(IEnumerable<CurrentTeam> teams)
        {
            return teams.First(t => t.Id == TeamIds.First());
        }

      
    }

    
}