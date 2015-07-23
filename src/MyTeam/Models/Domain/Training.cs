using System;

namespace MyTeam.Models.Domain
{
    public class Training : Event
    {
        public bool Recurring { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}