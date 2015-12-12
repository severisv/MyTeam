﻿using System;
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
        [Required]
        public string ShortName { get; set; }
        public int SortOrder { get; set; }
        [Required]
        public Guid ClubId { get; set; }
        public virtual Club Club { get; set; }
        public virtual ICollection<MemberTeam> MemberTeams { get; set; }
        public virtual ICollection<Season> Seasons { get; set; }

        [NotMapped]
        public virtual ICollection<Event> Events { get; set; }
        [NotMapped]
        public virtual IEnumerable<Event> Games => Events?.Where(e => e.IsGame);
        [NotMapped]
        public virtual IEnumerable<Event> Trainings => Events?.Where(e => e.IsTraining);
    }
}