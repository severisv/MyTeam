using System;

namespace MyTeam.Models.Domain
{
    public class MemberTeam : Entity
    {
        public Guid MemberId { get; set; }
        public Guid TeamId { get; set; }

        public virtual Team  Team { get; set; }
        public virtual Member Member { get; set; }
    }
}