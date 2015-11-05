using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MyTeam.Models.Enums;
using MyTeam.Resources;
using MyTeam.Settings;
    
namespace MyTeam.Models.Domain
{
    public class Player : Member
    {
        [Display(Name = Res.GameCount)]
        public virtual int GameCount { get; set; }
        [Display(Name = Res.GoalCount)]
        public virtual int GoalCount { get; set; }
        [Display(Name = Res.AssistCount)]
        public virtual int AssistCount { get; set; }

        public virtual int PracticeCount { get; set; }
        public PlayerStatus Status { get; set; }

        [Display(Name = Res.Positions)]
        public virtual IEnumerable<Position> Positions { get; set; }
        public virtual string PositionsToString => string.Join(", ", Positions.ToArray());

        public Player()
        {

        }

        public Player(string facebookId, string firstName, string lastName, string emailAddress)
        {
            FacebookId = facebookId;
            UserName = emailAddress;
            FirstName = firstName;
            MiddleName = "";
            LastName = lastName;
            Positions = new List<Position> { };
            RolesString = "";

        }
    }
}