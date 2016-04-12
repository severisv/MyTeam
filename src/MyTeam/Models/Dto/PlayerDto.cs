using System;


namespace MyTeam.Models.Dto
{
    public class PlayerDto
    {
        public Guid Id { get;  }
        public string Image { get;  }
        public string FirstName { get;  }
        public string FacebookId { get;  }
        public string[] Roles { get;  }
        public Guid[] TeamIds { get; }
        public bool ProfileIsConfirmed { get; }

        public PlayerDto(Guid playerId, string firstName, string image, string facebookId, string[] roles, Guid[] teamIds, bool profileIsConfirmed)
        {
            Id = playerId;
            Image = image;
            FacebookId = facebookId;
            Roles = roles;
            TeamIds = teamIds;
            ProfileIsConfirmed = profileIsConfirmed;
            FirstName = firstName;
        }
    }
}