using System;
using System.Collections.Generic;
using System.Linq;
using MyTeam.Models.Dto;
using MyTeam.ViewModels.Payment;

namespace MyTeam.ViewModels.Fine
{
    public class ListPaymentViewModel
    {
        public IEnumerable<PaymentViewModel> Payments { get; }
        public IEnumerable<SimplePlayerDto> Players { get; }
        public SimplePlayerDto SelectedPlayer { get; }
        public int SelectedYear { get; }
        public string PaymentInfo { get; }
        public IEnumerable<int> Years { get; }

        public ListPaymentViewModel(IEnumerable<PaymentViewModel> payments, IEnumerable<SimplePlayerDto> players, IEnumerable<int> years, int selectedYear, SimplePlayerDto selectedPlayer, string paymentInfo)
        {
            Payments = payments;
            Players = players;
            SelectedYear = selectedYear;
            Years = years;
            SelectedPlayer = selectedPlayer;
            PaymentInfo = paymentInfo;
        }


    }
}