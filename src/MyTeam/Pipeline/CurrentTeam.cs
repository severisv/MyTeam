using System;

namespace MyTeam
{
    public class CurrentTeam
    {
        public Guid Id { get; }
        public string ShortName { get; }

        public CurrentTeam(Guid id, string shortName)
        {
            Id = id;
            ShortName = shortName;
        }
    }
}