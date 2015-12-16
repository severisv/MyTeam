using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyTeam.Resources;

namespace MyTeam.Models.Domain
{
    public class Member : Entity
    {
        [Required]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public Guid ClubId { get; set; }

        public string UserName { get; set; }
        public string FacebookId { get; set; }

        public string RolesString { get; set; }

        [NotMapped]
        public string[] Roles => string.IsNullOrWhiteSpace(RolesString) ? new string[0] : RolesString.Split(',');

        [DataType(DataType.Date)]
        [Display(Name = Res.BirthDate)]
        public DateTime? BirthDate { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = Res.StartDate)]
        public DateTime? StartDate { get; set; }
        
        [DataType(DataType.PhoneNumber)]
        [Display(Name = Res.Phone)]
        public string Phone { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = Res.Email)]
        [NotMapped]
        public string Email => UserName;

        [NotMapped]
        public int? StartYear => StartDate?.Year;
        public string Imagename { get; set; }

        public string ImageSmall { get; set; }
        public string ImageMedium { get; set; }
        public string ImageFull { get; set; }

        public bool ProfileIsConfirmed { get; set; }

        [NotMapped]
        public string Fullname => $"{FirstName} {MiddleName} {LastName}";
        [NotMapped]
        public string Name => $"{FirstName} {LastName}";

        public virtual Club Club { get; set; }
        public virtual ICollection<Article> Articles { get; set; }
        public virtual ICollection<EventAttendance> EventAttendances { get; set; }
        public virtual ICollection<MemberTeam> MemberTeams { get; set; }
  
    }
}