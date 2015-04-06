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
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

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

        public string Email { get; set; }


        public int StartYear => StartDate.Year;
        public string Imagename { get; set; }

        [Display(Name = Res.Positions)]
        public virtual IEnumerable<Position> Positions { get; set; }
        public virtual string PositionsToString => string.Join(", ", Positions.ToArray());

        public string ImageSmall => Config.PlayerImages(Club.ShortName, Imagename, ImageSize.Small);
        public string ImageMedium => Config.PlayerImages(Club.ShortName, Imagename, ImageSize.Medium);
        public string ImageFull => Config.PlayerImages(Club.ShortName, Imagename, ImageSize.Full);
        public string Fullname => string.Format("{0} {1} {2}", FirstName, MiddleName, LastName);
        public string Name => string.Format("{0} {1}", FirstName, LastName);

        public virtual Club Club { get; set; }

        [Display(Name = Res.GameCount)]
        public virtual int GameCount { get; set; }
        [Display(Name = Res.GoalCount)]
        public virtual int GoalCount { get; set; }
        [Display(Name = Res.AssistCount)]
        public virtual int AssistCount { get; set; }

        public virtual int PracticeCount { get; set; }
        public PlayerStatus Status { get; set; }
    }

    public enum PlayerStatus
    {
        Aktiv,
        Inaktiv,
        Pensjonert
    }
}