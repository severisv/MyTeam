using System;

namespace MyTeam.Models.Domain
{
    public abstract class Entity
    {
        public Guid Id { get; internal set; }

        protected Entity(Guid? id = null)
        {
            Id = id ?? Guid.NewGuid();
        }
    }
}