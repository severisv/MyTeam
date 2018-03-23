using System;
using System.ComponentModel.DataAnnotations;

namespace MyTeam.Models.Domain
{
    public class EventTeam : Entity
    {
        [Required]
        public Guid TeamId { get; set; }
        [Required]
        public Guid EventId { get; set; }

        public virtual Event Event {get; set;}
        public virtual Team Team {get; set;}
    }
}