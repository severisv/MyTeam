using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyTeam.Models.Domain
{
    public class EventAttendance : Entity
    {
        [Required]
        public Guid EventId { get; set; }
        [Required]
        public Guid MemberId { get; set; }
        [Required]
        public bool IsAttending { get; set; }
        public bool DidAttend { get; set; }
        public virtual Member Member { get; set; }
        public virtual Event Event { get; set; }


    }
}