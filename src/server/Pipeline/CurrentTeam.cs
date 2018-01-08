using System;

namespace MyTeam
{
    public class CurrentTeam
    {
        public Guid Id { get; }
        public string ShortName { get; }
        public string Name { get; set; }

        public CurrentTeam(Guid id, string shortName, string name)
        {
            Id = id;
            ShortName = shortName;
            Name = name;
        }
    }
}