using System;

namespace MyTeam.ViewModels.Player
{
    public class GameAttendanceViewModel
    {
        public int Attendances { get; set; }
        public Guid TeamId { get; set; }
        public DateTime DateTime { get; set; }
    }
}