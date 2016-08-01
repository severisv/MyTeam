using System.Collections.Generic;
using System.Linq;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;

namespace MyTeam.ViewModels.Attendance
{
    public class AttendanceViewModel
    {
        public IEnumerable<int> Years { get; set; }

        public IEnumerable<PlayerAttendanceViewModel> Players
        {
            get
            {
                var players = _attendance.GroupBy(attendance => attendance.MemberId).Select(group => group.First().Member);
                return players.Select(player => new PlayerAttendanceViewModel
                {
                    PlayerId = player.Id,
                    Name = player.Name,
                    Trainings = _attendance.Where(a => a.DidAttend && a.MemberId == player.Id).Count(a => a.EventType == EventType.Trening),
                    Games = _attendance.Count(a => a.MemberId == player.Id && a.EventType == EventType.Kamp && a.IsSelected),
                    NoShows = _attendance.Where(a => a.IsAttending == true && !a.DidAttend && a.MemberId == player.Id).Count(a => a.EventType == EventType.Trening),
                    TrainingVictories = _attendance.Where(a => a.MemberId == player.Id).Count(a => a.WonTraining),
                    Image =  player.Image,
                    FacebookId = player.FacebookId,
                    UrlName = player.UrlName

                }).ToList()
                .Where(p => p.Games > 0 || p.Trainings > 0 || p.NoShows > 0)
                .OrderByDescending(p => p.Trainings)
                .ThenByDescending(p => p.TrainingVictories)
                .ThenByDescending(p => p.Games)
                .ThenByDescending(p => p.NoShows);
            }
        }

        public int Year { get; set; }
        
        private readonly IEnumerable<EventAttendanceViewModel> _attendance;


        public AttendanceViewModel(IEnumerable<EventAttendanceViewModel> attendance, IEnumerable<int> years, int year)
        {
            Year = year;
            Years = years;
            _attendance = attendance;
        }

     
      
    }
}