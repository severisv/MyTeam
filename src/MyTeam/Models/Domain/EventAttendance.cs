using System;

namespace MyTeam.Models.Domain
{
    public class EventAttendance : Entity
    {
        public Guid EventId { get; set; }
        public Guid PlayerId { get; set; }
        public bool IsAttending { get; set; }
        public virtual Player Player { get; set; }
        public virtual Event Event { get; set; }


    }
}