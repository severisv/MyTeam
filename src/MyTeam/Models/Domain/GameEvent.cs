using System;
using System.ComponentModel.DataAnnotations;
using MyTeam.Models.Enums;

namespace MyTeam.Models.Domain
{
    public class GameEvent : Entity
    {
        [Required]
        public Guid GameId { get; set; }
        public Guid? PlayerId { get; set; }
        public Guid? AssistedById { get; set; }
        public GameEventType Type { get; set; }
        public virtual Player AssistedBy { get; set; }
        public int TimeInMinutes { get; set; }
        public virtual Game Game { get; set; }
        public virtual Player Player { get; set; }

       }
}