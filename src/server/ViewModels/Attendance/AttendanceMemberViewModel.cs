using System;
using MyTeam.Models.Enums;

namespace MyTeam.ViewModels.Attendance
{
    public class AttendanceMemberViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string UrlName { get; set; }
        public string Image { get; set; }
        public string FacebookId { get; set; }
        public PlayerStatus Status { get; set; }

    }

}