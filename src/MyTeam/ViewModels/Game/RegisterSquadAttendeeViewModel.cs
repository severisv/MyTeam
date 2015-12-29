using System;

namespace MyTeam.ViewModels.Game
{
    public class RegisterSquadAttendeeViewModel
    {
        public Guid MemberId { get; set; }
        public bool IsSelected { get; set; }
        public string SignupMessage { get; set; }
        public bool IsAttending { get; set; }
    }
}