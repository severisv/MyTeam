using System;

namespace MyTeam.ViewModels.Events
{
    public class RegisterAttendanceAttendeeViewModel
    {
        public Guid MemberId { get; set; }
        public bool IsAttending { get; set; }
        public bool DidAttend { get; set; }
    }
}