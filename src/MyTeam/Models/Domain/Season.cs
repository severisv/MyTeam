using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyTeam.Models.Domain
{
    public class Season : Entity
    {
        [Required]
        public Guid TeamId { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [NotMapped]
        public string Name => StartDate.Year == EndDate.Year ? EndDate.Year.ToString() : $"{StartDate.Year} / {EndDate.Year}";

        public virtual Team Team { get; set; }
        public virtual IEnumerable<Table> Tables { get; set; }


      

    }
}