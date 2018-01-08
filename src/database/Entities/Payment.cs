using System;

namespace MyTeam.Models.Domain
{
    public class Payment : Entity
    {
        public Guid MemberId { get; set; }
        public Guid ClubId { get; set; }
        public int Amount { get; set; }
        public string Comment { get; set; }
        public DateTime TimeStamp { get; set; }
        public virtual Member Member { get; set; }
        public virtual Club Club { get; set; }
    }
}