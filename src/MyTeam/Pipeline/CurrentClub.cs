using System;
using System.Collections.Generic;
using System.Linq;
using MyTeam.Models.Dto;

namespace MyTeam
{
    public class CurrentClub
    {

        public Guid Id { get; }
        public string ClubId { get; }
        public string ShortName { get; }
        public string Name { get; }
        public IEnumerable<Guid> TeamIds => Teams.Select(t => t.Id);
        public IEnumerable<CurrentTeam> Teams { get; }
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
                Favicon = club.Favicon;
                Logo = club.Logo;
                Teams = club.Teams.Select(t => new CurrentTeam(t.Id, t.ShortName));
            }
          

        }

        public string GetTeamName(Guid teamId)
        {
            return Teams.SingleOrDefault(t => t.Id == teamId)?.ShortName;
        }
    }
}