using System.Collections.Generic;
using System.Linq;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;

namespace MyTeam.ViewModels.Member
{
    public class MemberListViewModel
    {
        public IEnumerable<MemberInfoViewModel> Members { get; }
        public PlayerStatus MemberStatus { get; }

        public MemberListViewModel(IEnumerable<MemberInfoViewModel> members, PlayerStatus memberStatus)
        {
            MemberStatus = memberStatus;
            Members = members;
        }
    }
}