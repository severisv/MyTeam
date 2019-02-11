using System;
using System.ComponentModel.DataAnnotations;

namespace MyTeam.Models.Domain
{
    public class EventAttendance : Entity
    {
        [Required]
        public Guid EventId { get; set; }
        [Required]
        public Guid MemberId { get; set; }
        public bool? IsAttending { get; set; }
        public bool DidAttend { get; set; }
        public bool IsSelected { get; set; }
        public virtual Member Member { get; set; }
        public virtual Event Event { get; set; }
        public string SignupMessage { get; set; }
        public bool WonTraining { get; set; }
    }
}