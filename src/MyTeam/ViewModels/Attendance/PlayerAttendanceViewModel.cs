using System;

namespace MyTeam.ViewModels.Attendance
{
    public class PlayerAttendanceViewModel
    {
        public Guid PlayerId { get; set; }
        public string Name { get; set; }
        public string UrlName { get; set; }
        public int Trainings { get; set; }
        public int Games { get; set; }
        public int NoShows { get; set; }
        public string Image { get; set; }
        public int TrainingVictories { get; set; }
        public string FacebookId { get; set; }
    }
}