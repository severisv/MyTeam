using System;
using MyTeam.Models.Enums;

namespace MyTeam.ViewModels.Member
{
    public class MemberInfoViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImageSmall { get; set; }
        public PlayerStatus Status { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}