using System;
using System.ComponentModel.DataAnnotations;

namespace MyTeam.Models.Domain
{
    public class GameEvent : Entity
    {
        public Guid? PlayerId { get; set; }
        [Required]
        public Guid GameId { get; set; }
        public int TimeInMinutes { get; set; }
        public virtual Game Game { get; set; }
        public virtual Player Player { get; set; }

    }
}