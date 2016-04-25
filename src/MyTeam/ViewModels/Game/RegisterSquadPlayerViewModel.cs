using System;
using MyTeam.Models.Dto;
using MyTeam.ViewModels.Events;

namespace MyTeam.ViewModels.Game
{
    public class RegisterSquadPlayerViewModel : SimplePlayerDto
    {
        public RegisterSquadAttendeeViewModel Attendance { get; }
        public Guid EventId { get; }

        public RegisterSquadPlayerViewModel(SimplePlayerDto player, Guid eventId, RegisterSquadAttendeeViewModel attendance)
        {
            Attendance = attendance;
            Id = player.Id;
            FirstName = player.FirstName;
            MiddleName = player.MiddleName;
            LastName = player.LastName;
            ImageFull = player.ImageFull;
            Status = player.Status;
            EventId = eventId;
            TeamIds = player.TeamIds;
            FacebookId = player.FacebookId;
        }
    }
}