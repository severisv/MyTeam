using System;
using System.Collections.Generic;
using System.Linq;
using MyTeam.ViewModels.Payment;

namespace MyTeam.ViewModels.Fine
{
    public class IndexViewModel
    {
        private readonly IEnumerable<FineViewModel> _fines;
        private readonly IEnumerable<PaymentViewModel> _payments;
        public int SelectedYear { get; }
        public IEnumerable<int> Years { get; }
        public IEnumerable<FineSummary> FineSummaries => _fines.GroupBy(f => f.MemberId).Select(g => new FineSummary(g, _payments.Where(p => p.MemberId == g.Key))).OrderByDescending(f => f.Total);

        public PaymentInfoViewModel PaymentInfo { get; }

        public double TotalSum => FineSummaries.Sum(f => f.Total);
        public IndexViewModel(IEnumerable<int> years, int selectedYear, IEnumerable<FineViewModel> fines, IEnumerable<PaymentViewModel> payments, PaymentInfoViewModel paymentInfo)
        {
            Years = years;
            SelectedYear = selectedYear;
            _fines = fines;
            _payments = payments;
            PaymentInfo = paymentInfo;
        }
    }
}