using System;
using System.Collections.Generic;
using MyTeam.Models.Dto;

namespace MyTeam
{
    public class CurrentClub
    {

        public Guid Id { get; }
        public string ClubId { get; }
        public string ShortName { get; }
        public string Name { get; }
        public IEnumerable<Guid> TeamIds { get; }
        public string Favicon { get; }
        public string Logo { get; }

        public CurrentClub(ClubDto club)
        {
            if (club != null)
            {
                Id = club.Id;
                ClubId = club.ClubId;
                Name = club.Name;
                ShortName = club.ShortName;
                TeamIds = club.TeamIds;
                Favicon = club.Favicon;
                Logo = club.Logo;
            }
          

        }
    }
}