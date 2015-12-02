using System;

namespace MyTeam.ViewModels.Events
{
    public class AttendeeViewModel
    {
       public string UserName { get;  }
       public string FirstName { get;  }
       public string LastName { get;  }
        public bool IsAttending { get; set; }
        public Guid Id { get; }
        public string Name => $"{FirstName} {LastName}";

        public AttendeeViewModel(Guid id, string firstName, string lastName, string username, bool isAttending)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            UserName = username;
            IsAttending = isAttending;
        }
    }
}