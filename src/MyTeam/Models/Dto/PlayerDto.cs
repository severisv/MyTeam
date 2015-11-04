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

        public PlayerDto(Guid playerId, string[] roles)
        {
            Id = playerId;
            Roles = roles;

        }
    }
}