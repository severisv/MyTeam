using System.Collections.Generic;
using System.Linq;
using MyTeam.Models.Dto;
using MyTeam.Models.Enums;

namespace MyTeam.ViewModels.Events
{
    public class RegisterAttendanceViewModel
    {
        public IEnumerable<SimpleEventViewModel>  PreviousEvents { get; } 
        public RegisterAttendanceEventViewModel Training { get; }
        private readonly IEnumerable<RegisterAttendancePlayerViewModel> _players;

        public IEnumerable<RegisterAttendancePlayerViewModel> Attendees => _players.Where(p => Training.Attendees.Any(a => a.MemberId == p.Id && a.IsAttending == true));

        public IEnumerable<RegisterAttendancePlayerViewModel> OtherActivePlayers => _players.Where(p => p.Status == PlayerStatus.Aktiv).Where(p => Attendees.All(a => a.Id != p.Id));

        public IEnumerable<RegisterAttendancePlayerViewModel> OtherInactivePlayers => _players.Where(p => Attendees.All(a => a.Id != p.Id)).Where(p => OtherActivePlayers.All(a => a.Id != p.Id));

        public RegisterAttendanceViewModel(RegisterAttendanceEventViewModel training, IEnumerable<SimplePlayerDto> players, IEnumerable<SimpleEventViewModel> previousEvents)
        {
            Training = training;
            _players = players.Select(p => new RegisterAttendancePlayerViewModel(p, Training.Id,
                    training.Attendees.FirstOrDefault(a => a.MemberId == p.Id)
                ));
          
            PreviousEvents = previousEvents;
        }

       

      
    }
}