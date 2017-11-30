using System;
using MyTeam.Models.Enums;

namespace MyTeam.ViewModels.Events
{
    public class AttendeePlayerViewModel
    {
        public Guid Id { get; }
        public string UserName { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string UrlName { get; }
        public PlayerStatus Status { get; }

        public AttendeePlayerViewModel(Guid id, string firstName, string lastName, string userName, string urlName, PlayerStatus status)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            UserName = userName;
            UrlName = urlName;
            Status = status;
        }
    }
}