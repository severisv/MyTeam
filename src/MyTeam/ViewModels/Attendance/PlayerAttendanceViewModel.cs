using System;

namespace MyTeam.ViewModels.Attendance
{
    public class PlayerAttendanceViewModel
    {
        public Guid PlayerId { get; set; }
        public string Name { get; set; }
        public int Trainings { get; set; }
        public int Games { get; set; }
        public int NoShows { get; set; }
        public string ImageSmall { get; set; }
        public int TrainingVictories { get; set; }
    }
}