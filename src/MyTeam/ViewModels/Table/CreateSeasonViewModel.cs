using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MyTeam.Validation.Attributes;

namespace MyTeam.ViewModels.Table
{
    public class CreateSeasonViewModel
    {
        public IEnumerable<TeamViewModel> Teams { get; set; }
        [RequiredNO]
        public Guid TeamId { get; set; }
        [RequiredNO]
        [Range(1970, 2170, ErrorMessage = "Årstallet må være et troverdig tall")]
        [Display(Name = "År")]
        public int Year { get; set; }
        [RequiredNO]
        [Display(Name = "Tittel")]
        public string Name { get; set; }
    }
}