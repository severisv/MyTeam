using System;
using MyTeam.Models.Enums;

namespace MyTeam.ViewModels.Member
{
    public class MemberInfoViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public PlayerStatus Status { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string FacebookId { get; set; }
        public DateTime? BirthDate { get; set; }
        public string BirthYear => BirthDate?.Year.ToString() ?? "";
    }
}