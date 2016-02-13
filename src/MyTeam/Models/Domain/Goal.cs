using System;

namespace MyTeam.Models.Domain
{
    public class Goal : GameEvent
    {
        public Guid? AssistedById { get; set; }
        public virtual Player AssistedBy { get; set; }
    }
}