using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MyTeam.Resources;
using MyTeam.Validation.Attributes;

namespace MyTeam.ViewModels.Table
{
    public class UpdateSeasonViewModel
    {
     
        public string TableString { get; set; }
        [Required]
        public Guid SeasonId { get; set; }


        [RequiredNO]
        public string Name { get; set; }

        public bool AutoUpdate { get; set; }

        [Url(ErrorMessage = "Lenken må være en gyldig url")]
        public string SourceUrl { get; set; }

        public bool AutoUpdateFixtures { get; set; }

        [Url(ErrorMessage = "Lenken må være en gyldig url")]
        public string FixturesSourceUrl { get; set; }
        public string Team { get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string DisplayNameShort => StartDate.Year == EndDate.Year ? $"{EndDate.Year}" : $"{StartDate.Year} / {EndDate.Year}";
  }
}
