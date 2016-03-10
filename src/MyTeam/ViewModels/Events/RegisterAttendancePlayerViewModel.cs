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
            FirstName = player.FirstName;
            MiddleName = player.MiddleName;
            LastName = player.LastName;
            ImageFull = player.ImageFull;
            Status = player.Status;
            EventId = eventId;
        }
    }
}