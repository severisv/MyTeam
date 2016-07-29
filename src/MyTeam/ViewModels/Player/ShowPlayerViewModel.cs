using System;
using System.ComponentModel.DataAnnotations;
using MyTeam.Models.Enums;
using MyTeam.Resources;

namespace MyTeam.ViewModels.Player
{
    public class ShowPlayerViewModel
    {
        public Guid Id { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = Res.BirthDate)]
        public DateTime? BirthDate { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string UrlName { get; set; }

        public PlayerStatus Status { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = Res.StartDate)]
        public DateTime? StartDate { get; set; }
        [DataType(DataType.PhoneNumber)]
        [Display(Name = Res.Phone)]
        public string Phone { get; set; }
        [DataType(DataType.EmailAddress)]
        [Display(Name = Res.Email)]
        public string Email => UserName;

        public int? StartYear => StartDate?.Year;
        public string ImageFull { get; set; }
        
        public string Fullname => $"{FirstName} {MiddleName} {LastName}";

        public int PracticeCount { get; set; }

        [Display(Name = Res.Positions)]
        public string[] Positions => PositionsString?.Split(',');
        public string PositionsString { get; set; }
        public string Name => $"{FirstName} {LastName}";
        public string FacebookId { get; set; }
       
    }
}


