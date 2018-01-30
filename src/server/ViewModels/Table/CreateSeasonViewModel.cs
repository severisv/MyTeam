﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MyTeam.Resources;
using MyTeam;

namespace MyTeam.ViewModels.Table
{
    public class CreateSeasonViewModel
    {
        public IList<TeamViewModel> Teams { get; set; }
        [RequiredNO]
        [Display(Name = Res.Team)]
        public Guid TeamId { get; set; }
        [RequiredNO]
        [Range(1970, 2170, ErrorMessage = "Årstallet må være et troverdig tall")]
        [Display(Name = "År")]
        public int? Year { get; set; }
        [RequiredNO]
        [Display(Name = "Tittel")]
        public string Name { get; set; }
        [Display(Name = "Automatisk oppdatering")]
        public bool AutoUpdate { get; set; }
        [Url(ErrorMessage = "Lenken må være en gyldig url")]
        public string SourceUrl { get; set; }

        [Display(Name = "Automatisk oppdatering")]
        public bool AutoUpdateFixtures { get; set; }
        [Url(ErrorMessage = "Lenken må være en gyldig url")]
        public string FixturesSourceUrl { get; set; }
    }
}