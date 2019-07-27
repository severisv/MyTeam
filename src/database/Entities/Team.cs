using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MyTeam.Models.Domain
{

    public enum Formasjon
    {
        FireFireTo = 442,
        FireTreTre = 433,
        TreToEn = 321,
        ToTreEn = 231,
    }
    
    public class Team : Entity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string ShortName { get; set; }
        public int SortOrder { get; set; }
        [Required]
        public Guid ClubId { get; set; }
        
        public Formasjon Formation { get; set; }
        public virtual Club Club { get; set; }
        public virtual ICollection<MemberTeam> MemberTeams { get; set; }
        public virtual ICollection<Season> Seasons { get; set; }

        public virtual ICollection<Game> Games { get; set; }
        public virtual ICollection<EventTeam> EventTeams { get; set; }
        [NotMapped]
        public virtual IEnumerable<Event> Trainings => EventTeams?.Select(e => e.Event).Where(e => e.IsTraining);
    }
}