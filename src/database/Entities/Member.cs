using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [DataType(DataType.Date)]
        [Display(Name = "Født")]
        public DateTime? BirthDate { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Signerte for klubben")]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Telefon")]
        public string Phone { get; set; }

        public string ImageFull { get; set; }
        public bool ProfileIsConfirmed { get; set; }
       
        public virtual ICollection<Article> Articles { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<EventAttendance> EventAttendances { get; set; }
        public virtual ICollection<MemberTeam> MemberTeams { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        
        public virtual string PositionsString { get; set; }
        public virtual ICollection<GameEvent> Assists { get; set; }
        public virtual ICollection<GameEvent> GameEvents { get; set; }

    }

}

