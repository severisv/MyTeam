using System;

namespace MyTeam.ViewModels.Events
{
    public class AttendeePlayerViewModel
    {
        public Guid Id { get; }
        public string UserName { get; }
        public string FirstName { get; }
        public string LastName { get; }

        public AttendeePlayerViewModel(Guid id, string firstName, string lastName, string userName)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            UserName = userName;
        }
    }
}