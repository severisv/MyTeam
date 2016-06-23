using System;

namespace MyTeam.Models.Domain
{
    public class Fine : Entity
    {
        public Guid MemberId { get; set; }
        public Guid RemedyRateId { get; set; }
        public int Amount { get; set; }
        public DateTime Issued { get; set; }
        public DateTime? Paid { get; set; }
        public string Comment { get; set; }
        public string RateName { get; set; }
        public virtual Member Member { get; set; }
        public virtual RemedyRate Rate { get; set; }
    }
}