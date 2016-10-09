using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MyTeam.Models.Dto;
using MyTeam.Validation.Attributes;
using MyTeam.ViewModels.RemedyRate;

namespace MyTeam.ViewModels.Payment
{
    public class AddPaymentViewModel
    {
        [RequiredNO]
        public Guid? MemberId { get; set; }

        [Display(Name = "Dato")]
        public DateTime? Date { get; set; }

        [Display(Name = "Beløp")]
        public int? Amount { get; set; }

        [Display(Name = "Kommentar")]
        public string Comment { get; set; }

        public IEnumerable<SimplePlayerDto> Players { get; set; }

        public AddPaymentViewModel() { }
        public AddPaymentViewModel(IEnumerable<SimplePlayerDto> players)
        {
            Players = players;
        }

    }
}