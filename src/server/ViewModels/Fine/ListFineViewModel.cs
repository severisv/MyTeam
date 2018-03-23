using System;
using System.Collections.Generic;
using System.Linq;
using MyTeam.Models.Dto;
using MyTeam.ViewModels.RemedyRate;

namespace MyTeam.ViewModels.Fine
{
    public class ListFineViewModel
    {
        public IEnumerable<FineViewModel> Fines { get; }
        public IEnumerable<RemedyRateViewModel> Rates { get; }
        public IEnumerable<SimplePlayerDto> Players { get; }
        public SimplePlayerDto SelectedPlayer { get; }
        public int SelectedYear { get; }
        public IEnumerable<int> Years { get; }

        public ListFineViewModel(IEnumerable<FineViewModel> fines, IEnumerable<RemedyRateViewModel> rates, IEnumerable<SimplePlayerDto> players, IEnumerable<int> years, int selectedYear, SimplePlayerDto selectedPlayer)
        {
            Fines = fines;
            Rates = rates;
            Players = players;
            SelectedYear = selectedYear;
            Years = years;
            SelectedPlayer = selectedPlayer;
        }


    }
}