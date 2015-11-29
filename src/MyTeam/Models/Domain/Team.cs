using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MyTeam.Models.Domain
{
    public class Team : Entity
    {
        [Required]
        public string Name { get; set; }
        public int SortOrder { get; set; }
        [Required]
        public Guid ClubId { get; set; }
        public virtual Club Club { get; set; }
//        public virtual IEnumerable<Member> Members { get; set; }

        [NotMapped]
        public virtual IList<Event> Events { get; set; }
        [NotMapped]
        public virtual IEnumerable<Event> Games => Events?.Where(e => e.IsGame);
        [NotMapped]
        public virtual IEnumerable<Event> Trainings => Events?.Where(e => e.IsTraining);
    }
}