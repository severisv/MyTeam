using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MyTeam.Models.Enums;
using MyTeam.Resources;
using MyTeam.Settings;
    
namespace MyTeam.Models.Domain
{
    public class Player : Entity
    {
        [Required]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }

        public string UserName { get; set; }
        public string FacebookId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = Res.BirthDate)]
        public DateTime BirthDate { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = Res.StartDate)]
        public DateTime StartDate { get; set; }


        [DataType(DataType.PhoneNumber)]
        [Display(Name = Res.Phone)]
        public int Phone { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = Res.Email)]
        public string Email => UserName;


        public int StartYear => StartDate.Year;
        public string Imagename { get; set; }

        [Display(Name = Res.Positions)]
        public virtual IEnumerable<Position> Positions { get; set; }
        public virtual string PositionsToString => string.Join(", ", Positions.ToArray());

        public string ImageSmall => Config.PlayerImages(Club.ShortName, Imagename, ImageSize.Small);
        public string ImageMedium => Config.PlayerImages(Club.ShortName, Imagename, ImageSize.Medium);
        public string ImageFull => Config.PlayerImages(Club.ShortName, Imagename, ImageSize.Full);
        public string Fullname => $"{FirstName} {MiddleName} {LastName}";
        public string Name => $"{FirstName} {LastName}";

        public virtual Club Club { get; set; }

        [Display(Name = Res.GameCount)]
        public virtual int GameCount { get; set; }
        [Display(Name = Res.GoalCount)]
        public virtual int GoalCount { get; set; }
        [Display(Name = Res.AssistCount)]
        public virtual int AssistCount { get; set; }

        public virtual int PracticeCount { get; set; }
        public PlayerStatus Status { get; set; }


        public Player()
        {
            
        }

        public Player(string facebookId, string firstName, string lastName)
        {

            FirstName = firstName;
            MiddleName = "";
            LastName = lastName;
            Positions = new List<Position> {};

        }
    }
}