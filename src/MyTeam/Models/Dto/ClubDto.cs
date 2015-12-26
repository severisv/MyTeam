using System;
using System.Collections.Generic;
using MyTeam.Models.Domain;

namespace MyTeam.Models.Dto
{
    public class ClubDto
    {
        public Guid Id { get; }
        public string ClubId { get; }
        public string ShortName { get; }
        public string Name { get; }
        public IEnumerable<Guid> TeamIds { get; }
        public IEnumerable<TeamDto> Teams { get; }
        public string Favicon { get;  }
        public string Logo { get;  }


        public ClubDto(Guid id, string clubId, string name, string shortName, string logo, string favicon, IEnumerable<TeamDto> teams)
        {
            Id = id;
            ClubId = clubId;
            Name = name;
            ShortName = shortName;
            Teams = teams;
            Favicon = favicon;
            Logo = logo;
        }
    }
}