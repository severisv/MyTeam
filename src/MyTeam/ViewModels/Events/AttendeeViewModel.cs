using System;

namespace MyTeam.ViewModels.Events
{
    public class AttendeeViewModel
    {
        public string UserName => Player?.UserName;
        public string FirstName => Player?.FirstName;
        public string LastName => Player?.LastName;
        public AttendeePlayerViewModel Player { get; set; }
        public bool? IsAttending { get; set; }
        public bool IsSelected { get; }
        public bool DidAttend { get; }
        public Guid MemberId { get; }
        public Guid EventId { get; }
        public string Name => $"{FirstName} {LastName}";
        public string SignupMessage { get; set; }
        public string LastInitials => $"{LastName.Substring(0,1)}";

        public AttendeeViewModel(Guid memberId, Guid eventId, string signupMessage, bool? isAttending, bool? didAttend, bool? isSelected, AttendeePlayerViewModel player = null)
        {
            EventId = eventId;
            MemberId = memberId;
            IsAttending = isAttending;
            DidAttend = didAttend ?? false;
            IsSelected = isSelected ?? false;
            SignupMessage = signupMessage;
            Player = player;
        }
    }
}