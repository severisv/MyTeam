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


        public ClubDto(string clubId, string name, string shortName, IEnumerable<Guid> ids)
        {
            ClubId = clubId;
            Name = name;
            ShortName = shortName;
            TeamIds = ids;
        }
    }
}