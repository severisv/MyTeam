using System;
using System.ComponentModel.DataAnnotations;
using MyTeam.Resources;
using MyTeam.Validation.Attributes;

namespace MyTeam.ViewModels.RemedyRate
{
    public class RemedyRateViewModel
    {
        public Guid Id { get; set; }
        [RequiredNO]
        [Display(Name = Res.Name)]
        public string Name { get; set; }
        [Display(Name = Res.Description)]
        public string Description { get; set; }
        [RequiredNO]
        [Display(Name = "Sats")]
        [Range(1, 1000000, ErrorMessage = "Boten må være på mer enn 0")]
        public int? Rate { get; set; }

    }
}