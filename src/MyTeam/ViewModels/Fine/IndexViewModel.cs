using System;
using System.Collections.Generic;
using System.Linq;
using MyTeam.Models.Dto;
using MyTeam.ViewModels.RemedyRate;

namespace MyTeam.ViewModels.Fine
{
    public class IndexViewModel
    {

        public int SelectedYear { get; }
        public IEnumerable<int> Years { get; }

        public IndexViewModel(IEnumerable<int> years, int selectedYear)
        {
            Years = years;
            SelectedYear = selectedYear;
        }


    }
}