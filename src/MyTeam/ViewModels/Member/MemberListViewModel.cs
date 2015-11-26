using System.Collections.Generic;
using System.Linq;
using MyTeam.Models.Domain;

namespace MyTeam.ViewModels.Member
{
    public class MemberListViewModel
    {
        public IEnumerable<MemberInfoViewModel> Members { get; }

        public MemberListViewModel(IEnumerable<MemberInfoViewModel> members)
        {
            Members = members;
        }
    }
}