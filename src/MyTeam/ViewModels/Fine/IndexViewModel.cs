using System;
using System.Collections.Generic;
using System.Linq;

namespace MyTeam.ViewModels.Fine
{
    public class IndexViewModel
    {
        private readonly IEnumerable<FineViewModel> _fines;
        public int SelectedYear { get; }
        public IEnumerable<int> Years { get; }
        public IEnumerable<FineSummary> FineSummaries => _fines.GroupBy(f => f.MemberId).Select(g => new FineSummary(g)).OrderByDescending(f => f.Total);

        public PaymentInfoViewModel PaymentInfo { get; }

        public double TotalSum => FineSummaries.Sum(f => f.Total);
        public IndexViewModel(IEnumerable<int> years, int selectedYear, IEnumerable<FineViewModel> fines, PaymentInfoViewModel paymentInfo)
        {
            Years = years;
            SelectedYear = selectedYear;
            _fines = fines;
            PaymentInfo = paymentInfo;
        }


    }
}