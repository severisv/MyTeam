using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyTeam.Models.Enums;
using MyTeam.Resources;
    
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

        [Required]
        public PlayerStatus Status { get; set; }

        [Display(Name = Res.Positions)]
        [NotMapped]
        public string[] Positions => PositionsString?.Split(',');
        public virtual string PositionsString { get; set; }

        public virtual ICollection<Goal> Goals { get; set; }
        public virtual ICollection<Goal> Assists { get; set; }
        public virtual ICollection<Card> Cards { get; set; }

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
            PositionsString = "";
            RolesString = "";

        }
    }
}