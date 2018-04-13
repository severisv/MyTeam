using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyTeam.Models.Enums;
using MyTeam.Resources;
using MyTeam.Models.Shared;

namespace MyTeam.Models.Domain
{
    public class Member : Entity, IMember
    {
        [Required]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string UrlName { get; set; }
        [Required]
        public Guid ClubId { get; set; }

        public string UserName { get; set; }
        public string FacebookId { get; set; }

        public string RolesString { get; set; }


        [Required]
        public int Status { get; set; }
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

        public string ImageFull { get; set; }
        [NotMapped]
        public string Image => ImageFull;
        public bool ProfileIsConfirmed { get; set; }

        [NotMapped]
        public string Fullname => $"{FirstName} {MiddleName} {LastName}";
        [NotMapped]
        public string Name => $"{FirstName} {LastName}";

        public virtual ICollection<Article> Articles { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<EventAttendance> EventAttendances { get; set; }
        public virtual ICollection<MemberTeam> MemberTeams { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }

    }

}

