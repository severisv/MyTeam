using System;


namespace MyTeam.Models.Dto
{
    public class SquadMember
    {
        public Guid Id { get;  }
        public string Image { get;  }
        public string FirstName { get;  }
        public string LastName { get;  }
        public string FacebookId { get;  }

        public string GetName(bool hasDuplicate) => 
            $"{FirstName}{(hasDuplicate ? $" {LastName.Substring(0,1)}" : "")}";

        public SquadMember(
            Guid playerId, 
            string firstName, 
            string lastName, 
            string image, 
            string facebookId)
        {
            Id = playerId;
            Image = image;
            FacebookId = facebookId;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}