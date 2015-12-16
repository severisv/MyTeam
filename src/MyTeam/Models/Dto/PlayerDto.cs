using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
using MyTeam.Resources;
using MyTeam.Settings;

namespace MyTeam.Models.Dto
{
    public class PlayerDto
    {
        public Guid Id { get;  }
        public string[] Roles { get;  }
        public Guid[] TeamIds { get; }
        public bool ProfileIsConfirmed { get; }

        public PlayerDto(Guid playerId, string[] roles, Guid[] teamIds, bool profileIsConfirmed)
        {
            Id = playerId;
            Roles = roles;
            TeamIds = teamIds;
            ProfileIsConfirmed = profileIsConfirmed;
        }
    }
}