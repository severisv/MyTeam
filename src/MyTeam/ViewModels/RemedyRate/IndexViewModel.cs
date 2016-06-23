

using System.Collections.Generic;

namespace MyTeam.ViewModels.RemedyRate
{
    public class IndexViewModel
    {
        public IEnumerable<RemedyRateViewModel> RemedyRates { get; }
        public string PaymentInfo { get;  }

        public IndexViewModel(IEnumerable<RemedyRateViewModel> remedyRates, string paymentInfo)
        {
            PaymentInfo = paymentInfo;
            RemedyRates = remedyRates;
        }

    }
}