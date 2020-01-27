using System;


namespace MyTeam.Models.Dto
{
    public class PlayerDto
    {
        public Guid Id { get;  }
        public string Image { get;  }
        public string FirstName { get;  }
        public string UrlName { get;  }
        public string FacebookId { get;  }
        public string RolesString { get;  }
        public string[] Roles => string.IsNullOrWhiteSpace(RolesString) ? new string[0] : RolesString.Split(',');
        public Guid[] TeamIds { get; }
        public bool ProfileIsConfirmed { get; }

        public PlayerDto(Guid playerId, string firstName, string urlName, string image, string facebookId, string roles, Guid[] teamIds, bool profileIsConfirmed)
        {
            Id = playerId;
            Image = image;
            FacebookId = facebookId;
            RolesString = roles;
            TeamIds = teamIds;
            ProfileIsConfirmed = profileIsConfirmed;
            FirstName = firstName;
            UrlName = urlName;
        }
    }
}