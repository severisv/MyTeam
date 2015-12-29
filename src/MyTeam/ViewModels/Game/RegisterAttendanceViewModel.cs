using System.Collections.Generic;
using System.Linq;
using MyTeam.Models.Dto;
using MyTeam.Models.Enums;
using MyTeam.ViewModels.Events;

namespace MyTeam.ViewModels.Game
{
    public class RegisterSquadViewModel
    {
        public RegisterAttendanceEventViewModel Training { get; }
        private readonly IEnumerable<RegisterSquadPlayerViewModel> _players;

        public IEnumerable<RegisterSquadPlayerViewModel> Attendees => _players.Where(p => Training.Attendees.Any(a => a.MemberId == p.Id && a.IsAttending));

        public IEnumerable<RegisterSquadPlayerViewModel> OtherActivePlayers => _players.Where(p => p.Status == PlayerStatus.Aktiv).Where(p => Attendees.All(a => a.Id != p.Id));

        public IEnumerable<RegisterSquadPlayerViewModel> OtherInactivePlayers => _players.Where(p => Attendees.All(a => a.Id != p.Id)).Where(p => OtherActivePlayers.All(a => a.Id != p.Id));

        public IEnumerable<RegisterSquadPlayerViewModel> Squad => _players.Where(p => !p.Attendance?.IsSelected == true);

        public RegisterSquadViewModel(RegisterAttendanceEventViewModel training, IEnumerable<SimplePlayerDto> players)
        {
            Training = training;
            _players = players.Select(p => new RegisterSquadPlayerViewModel(p, Training.Id,
                    training.Attendees.FirstOrDefault(a => a.MemberId == p.Id)
                ));
        }
    }
}