using System;
using System.Linq;
using Microsoft.AspNet.Authentication.Notifications;
using MyTeam.Models.Dto;
using MyTeam.Models.Enums;

namespace MyTeam
{
    public class UserMember
    {

    

        public Guid Id { get; }
        public string[] Roles { get; }


        public UserMember(PlayerDto player)
        {
            if (player != null)
            {
                Id = player.Id;
                Roles = player.Roles;
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

        public bool Exists => Id != Guid.Empty;

    }
}