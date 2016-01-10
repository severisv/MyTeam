using System;
using MyTeam.Models.Dto;

namespace MyTeam.ViewModels.Events
{
    public class RegisterAttendancePlayerViewModel : SimplePlayerDto
    {
        public RegisterAttendanceAttendeeViewModel Attendance { get; }
        public Guid EventId { get; }

        public RegisterAttendancePlayerViewModel(SimplePlayerDto player, Guid eventId, RegisterAttendanceAttendeeViewModel attendance)
        {
            Attendance = attendance;
            Id = player.Id;
            Name = player.Name;
            ImageSmall = player.ImageSmall;
            Status = player.Status;
            EventId = eventId;
        }
    }
}