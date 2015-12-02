using System;

namespace MyTeam.ViewModels.Events
{
    public class AttendeeViewModel
    {
       public string UserName { get;  }
       public string FirstName { get;  }
       public string LastName { get;  }
        public bool IsAttending { get; set; }
        public bool DidAttend { get; set; }
        public Guid MemberId { get; }
        public Guid EventId { get; }
        public string Name => $"{FirstName} {LastName}";

        public AttendeeViewModel(Guid memberId, Guid eventId, string firstName, string lastName, string username, bool? isAttending, bool? didAttend)
        {
            EventId = eventId;
            MemberId = memberId;
            FirstName = firstName;
            LastName = lastName;
            UserName = username;
            IsAttending = isAttending ?? false;
            DidAttend = didAttend ?? false;
        }
    }
}