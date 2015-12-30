using System;

namespace MyTeam.ViewModels.Events
{
    public class AttendeeViewModel
    {
       public string UserName { get;  }
       public string FirstName { get;  }
       public string LastName { get;  }
        public bool IsAttending { get; set; }
        public bool IsSelected { get; }
        public bool DidAttend { get; }
        public Guid MemberId { get; }
        public Guid EventId { get; }
        public string Name => $"{FirstName} {LastName}";
        public string SignupMessage { get; set; }

        public AttendeeViewModel(Guid memberId, Guid eventId, string firstName, string lastName, string username, string signupMessage, bool? isAttending, bool? didAttend, bool? isSelected)
        {
            EventId = eventId;
            MemberId = memberId;
            FirstName = firstName;
            LastName = lastName;
            UserName = username;
            IsAttending = isAttending ?? false;
            DidAttend = didAttend ?? false;
            IsSelected = isSelected ?? false;
            SignupMessage = signupMessage;
        }
    }
}