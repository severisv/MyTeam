using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyTeam.Models.Domain
{
    public class Club : Entity
    {
        [Required]
        public string ClubIdentifier { get; set; }
        [Required]
        public string ShortName { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Logo { get; set; }
        public string Favicon { get; set; }
        public string Description { get; set; }
        public string Sponsors { get; set; }
        public virtual ICollection<Team> Teams { get; set; }
        public virtual ICollection<Member> Members { get; set; } 
        public virtual ICollection<Event> Events { get; set; } 
    }
}