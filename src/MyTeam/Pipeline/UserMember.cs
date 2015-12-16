using System;
using System.Linq;
using MyTeam.Models.Dto;

namespace MyTeam
{
    public class UserMember
    {
        public Guid Id { get; }
        public string[] Roles { get; }
        public Guid[] TeamIds { get; }
        public bool ProfileIsConfirmed { get; }


        public UserMember(PlayerDto player)
        {
            if (player != null)
            {
                Id = player.Id;
                Roles = player.Roles;
                TeamIds = player.TeamIds;
                ProfileIsConfirmed = player.ProfileIsConfirmed;
            }
            else
            {
                Roles = new string[] {};
            }
        }

        public bool IsSelfOrAdmin(Guid id)
        {
            if (id == Id) return true;

            return Roles.ContainsAny(Models.Enums.Roles.Admin , Models.Enums.Roles.Coach);
        }

        public bool BelongsToTeam(params Guid[] teamIds)
        {
            return TeamIds.Any(teamIds.Contains);
        }

        public bool Exists => Id != Guid.Empty;
    }
}