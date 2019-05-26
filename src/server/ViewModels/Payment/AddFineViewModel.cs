using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MyTeam.Models.Dto;

namespace MyTeam.ViewModels.Payment
{
    public class AddPaymentViewModel
    {
        [RequiredNO]
        public Guid? MemberId { get; set; }

        [Display(Name = "Dato")]
        public DateTime? Date { get; set; }

        [Display(Name = "Bel�p")]
        [RequiredNO]
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