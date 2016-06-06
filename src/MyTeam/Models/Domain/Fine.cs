using System;

namespace MyTeam.Models.Domain
{
    public class Fine : Entity
    {
        public RemedyRate Rate { get; set; }
        public int? ExtraRate { get; set; }
        public DateTime? Issued { get; set; }
        public Guid MemberId { get; set; }
        public virtual Member Member { get; set; }
    }
}