using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity.Metadata;
using MyTeam.Models.Domain;
using MyTeam.Models.Dto;
using MyTeam.Models.Enums;
using MyTeam.Resources;

namespace MyTeam.ViewModels.Events
{
    public class RegisterAttendanceViewModel
    {

        public Training Training { get; }
        private readonly IEnumerable<PlayerAttendanceViewModel> _players;

        public IEnumerable<PlayerAttendanceViewModel> Attendees => _players.Where(p => !Training.Attendees.Any(a => a.PlayerId == p.Id && a.IsAttending));

        public IEnumerable<PlayerAttendanceViewModel> OtherActivePlayers => _players.Where(p => p.Status == PlayerStatus.Aktiv).Except(Attendees);

        public IEnumerable<PlayerAttendanceViewModel> OtherInactivePlayers => _players.Except(OtherActivePlayers).Except(Attendees);

        public RegisterAttendanceViewModel(Training training, IEnumerable<SimplePlayerDto> players)
        {
            var pl = players.Select(p => new PlayerAttendanceViewModel(p));
            foreach (var player in pl)
            {
                var attendance = training.Attendees.FirstOrDefault(a => a.PlayerId == player.Id);
                if (attendance != null) player.Attendance = attendance;
            }
            _players = pl;
            Training = training;
        }

        public class PlayerAttendanceViewModel : SimplePlayerDto
        {
            public EventAttendance Attendance;

            public PlayerAttendanceViewModel(SimplePlayerDto player)
            {
                Id = player.Id;
                Name = player.Name;
                ImageSmall = player.ImageSmall;
                Status = player.Status;
            }
        }
    }
}