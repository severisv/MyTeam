using System;
using System.ComponentModel.DataAnnotations;
using MyTeam.Validation.Attributes;
using System.Collections.Generic;
using MyTeam.ViewModels.RemedyRate;
using MyTeam.Models.Dto;
using MyTeam.Resources;

namespace MyTeam.ViewModels.Fine
{
    public class AddFineViewModel
    {
        [RequiredNO]
        public Guid? MemberId { get; set; }
        [RequiredNO]
        public Guid? RateId { get; set; }

        [Display(Name = "Dato")]
        public DateTime? Date { get; set; }

        [Display(Name = "Tillegg")]
        public int? ExtraRate { get; set; }
        [Display(Name = "Kommentar")]
        public string Comment { get; set; }

        public IEnumerable<RemedyRateViewModel> Rates { get; set; }
        public IEnumerable<SimplePlayerDto> Players { get; set; }

        public AddFineViewModel() { }
        public AddFineViewModel(IEnumerable<RemedyRateViewModel> rates, IEnumerable<SimplePlayerDto> players)
        {
            Rates = rates;
            Players = players;
        }

    }
}