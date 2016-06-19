using System;
using System.Collections.Generic;
using MyTeam.Models.Dto;
using MyTeam.ViewModels.RemedyRate;

namespace MyTeam.ViewModels.Fine
{
    public class ListFineViewModel
    {
        public IEnumerable<FineViewModel> Fines { get; }
        public IEnumerable<RemedyRateViewModel> Rates { get; }
        public IEnumerable<SimplePlayerDto> Players { get; }

        public ListFineViewModel(IEnumerable<FineViewModel> fines, IEnumerable<RemedyRateViewModel> rates, IEnumerable<SimplePlayerDto> players)
        {
            Fines = fines;
            Rates = rates;
            Players = players;
        }


    }
}