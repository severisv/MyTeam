using System;
using System.ComponentModel.DataAnnotations;

namespace MyTeam.Models.Domain
{
    public class EventAttendance : Entity
    {
        [Required]
        public Guid EventId { get; set; }
        [Required]
        public Guid PlayerId { get; set; }
        [Required]
        public bool IsAttending { get; set; }
        public bool DidAttend { get; set; }
        public virtual Player Player { get; set; }
        public virtual Event Event { get; set; }


    }
}