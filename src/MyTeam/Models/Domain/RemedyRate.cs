using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyTeam.Models.Domain
{
    public class RemedyRate : Entity
    {
        [Required]
        public Guid ClubId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Rate { get; set; }

        public bool IsDeleted { get; set; }

        public virtual IEnumerable<Fine> Fines { get; set; }
    }
}