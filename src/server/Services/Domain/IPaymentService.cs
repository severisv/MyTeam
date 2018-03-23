using System;
using System.Collections.Generic;
using MyTeam.ViewModels.Fine;
using MyTeam.ViewModels.Payment;

namespace MyTeam.Services.Domain
{
    public interface IPaymentService
    {
        IEnumerable<PaymentViewModel> GetPayments(Guid clubId, int? year = null, Guid? memberId = null);
        void Delete(Guid paymentId);
        Guid Add(AddPaymentViewModel payment, Guid clubId);
        PaymentViewModel Get(Guid paymentId);
        IEnumerable<int> GetYears(Guid id);
    }

}