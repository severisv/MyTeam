using System;
using MyTeam.Models.Enums;

namespace MyTeam.ViewModels.Attendance
{
    public class EventAttendanceViewModel
    {
        public Guid MemberId { get; set; }
        public AttendanceMemberViewModel Member { get; set; }
        public bool DidAttend { get; set; }
        public EventType EventType { get; set; }
        public bool? IsAttending { get; set; }
        public bool WonTraining { get; set; }
    }
}