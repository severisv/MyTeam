using System;

namespace MyTeam.Models.Dto
{
    public class TeamDto
    {
        public Guid Id { get; }
        public string ShortName { get; }
        public string Name { get; set; }

        public TeamDto(Guid id, string shortName, string name)
        {
            Id = id;
            ShortName = shortName;
            Name = name;
        }
    }
}