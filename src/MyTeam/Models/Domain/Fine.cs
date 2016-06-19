using System;

namespace MyTeam.Models.Domain
{
    public class Fine : Entity
    {
        public Guid MemberId { get; set; }
        public Guid RemedyRateId { get; set; }
        public int? ExtraRate { get; set; }
        public DateTime? Issued { get; set; }
        public DateTime? Paid { get; set; }
        public virtual Member Member { get; set; }
        public virtual RemedyRate Rate { get; set; }

    }
}