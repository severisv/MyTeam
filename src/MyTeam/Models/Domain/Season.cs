using System;

namespace MyTeam.Models.Domain
{
    public class Season : Entity
    {
        public Guid TeamId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Name => StartDate.Year == EndDate.Year ? EndDate.Year.ToString() : $"{StartDate.Year} / {EndDate.Year}";

        public virtual Team Team { get; set; }
        public virtual Table Table { get; set; }


      

    }
}