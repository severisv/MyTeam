using System;
using System.Collections.Generic;
using MyTeam.Models.Domain;

namespace MyTeam.Models.Dto
{
    public class ClubDto
    {
        public string ClubId { get; }
        public string ShortName { get; }
        public string Name { get; }
        public IEnumerable<Guid> TeamIds { get; }
        public string Favicon { get;  }
        public string Logo { get;  }


        public ClubDto(string clubId, string name, string shortName, string logo, string favicon, IEnumerable<Guid> teamIds)
        {
            ClubId = clubId;
            Name = name;
            ShortName = shortName;
            TeamIds = teamIds;
            Favicon = favicon;
            Logo = logo;
        }
    }
}