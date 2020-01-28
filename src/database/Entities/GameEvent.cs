using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public virtual Member AssistedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public virtual Event Game { get; set; }
        public virtual Member Player { get; set; }

      }
}