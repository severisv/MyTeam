using System;

namespace MyTeam.Models.Dto
{
    public class TeamDto
    {
        public Guid Id { get; }
        public string ShortName { get; }

        public TeamDto(Guid id, string shortName)
        {
            Id = id;
            ShortName = shortName;
        }
    }
}