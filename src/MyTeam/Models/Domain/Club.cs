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
        public virtual List<Team> Teams { get; set; }
        public virtual List<Member> Players { get; set; } 
    }
}