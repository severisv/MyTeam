using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using MyTeam.Models.Enums;
using MyTeam.Models.Shared;

namespace MyTeam.ViewModels.Events
{
    public class SignupDetailsViewModel : IEvent
    {
        public Guid Id { get; }
        public EventType Type { get; }
        public GameType? GameType { get; }
        public DateTime DateTime { get;  }
        public bool Voluntary { get; }
        public IEnumerable<Guid> TeamIds { get; }

        public IList<AttendeeViewModel> Attendees => _attendees?.OrderBy(a => a.FirstName).ToList();
        private readonly IList<AttendeeViewModel> _attendees;

        public SignupDetailsViewModel(IEnumerable<Guid> teamIds, IEnumerable<AttendeeViewModel> attendees, Guid eventId, EventType type, 
            GameType? gameType, DateTime dateTime,
           bool voluntary, bool isPublished)
        {
            Id = eventId;
            _attendees = (attendees ?? Enumerable.Empty<AttendeeViewModel>()).ToList();
            DateTime = dateTime;
            Type = type;
            GameType = gameType;
            Voluntary = voluntary;
            TeamIds = teamIds;
            IsPublished = isPublished;
        }

        public IEnumerable<AttendeeViewModel> Attending => Attendees?.Where(a => a.IsAttending == true);
        public IEnumerable<AttendeeViewModel> NotAttending => Attendees?.Where(a => a.IsAttending == false);
        public IEnumerable<AttendeeViewModel> DidAttend => Attendees?.Where(a => a.DidAttend);
        public IEnumerable<AttendeeViewModel> Squad => Attendees?.Where(a => a.IsSelected);
        public IEnumerable<AttendeeViewModel> Coaches => Attendees?.Where(a => a.IsAttending == true && a.Player.Status == PlayerStatus.Trener);

        public bool IsGame => Type == EventType.Kamp;
        public bool IsTraining => Type == EventType.Trening;
        public bool IsCustom => Type == EventType.Diverse;
        public bool IsPublished { get;  }

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
            return Attendees?.FirstOrDefault(a => a.MemberId == memberId)?.SignupMessage;
        }

        public bool SignupHasOpened()
        {
            if (Type == EventType.Diverse) return true;
            if (Type == EventType.Kamp && GameType == Models.Enums.GameType.Treningskamp) return true;
            return DateTime.Date - DateTime.Now.Date < new TimeSpan(Settings.Config.AllowedSignupDays, 0, 0, 0, 0);
        }

     
        public void SetAttendance(AttendeeViewModel attendeeViewModel, bool isAttending)
        {
            var attendee = 
                Attendees.FirstOrDefault(a => a.MemberId == attendeeViewModel.MemberId);

            if (attendee != null)
            {
                attendee.IsAttending = isAttending;
            }
            else
                _attendees.Add(attendeeViewModel);

        }

        public CurrentTeam Team(IEnumerable<CurrentTeam> teams)
        {
            return teams.First(t => t.Id == TeamIds.First());
        }

      
    }

    
}