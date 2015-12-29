using System;
using MyTeam.Models.Dto;
using MyTeam.ViewModels.Events;

namespace MyTeam.ViewModels.Game
{
    public class RegisterSquadPlayerViewModel : SimplePlayerDto
    {
        public RegisterAttendanceAttendeeViewModel Attendance { get; }
        public Guid EventId { get; }

        public RegisterSquadPlayerViewModel(SimplePlayerDto player, Guid eventId, RegisterAttendanceAttendeeViewModel attendance)
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